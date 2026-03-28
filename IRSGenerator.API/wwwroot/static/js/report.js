// ── Constants ─────────────────────────────────────────────────────────────────
const DECISION_LABELS = {
  USE_AS_IS:     'Accept (Spec)',
  REWORK:        'Rework',
  MRB_SUBMITTED: 'MRB Submitted',
  MRB_CTP:       'MRB — CTP',
  MRB_ACCEPTED:  'MRB Accepted',
  MRB_REJECTED:  'MRB Rejected',
  VOID:          'Void',
  REPAIR:        'Repair',
  SCRAP:         'Scrap',
  PENDING:       'Pending',
};

const STATUS_LABELS = {
  open:      'Open',
  completed: 'Completed',
  rejected:  'Rejected',
};

// Landscape status badge class
function defStatusClass(decision) {
  if (!decision) return 'st-pending';
  if (['USE_AS_IS','MRB_ACCEPTED'].includes(decision)) return 'st-accepted';
  if (['REWORK','REPAIR'].includes(decision))          return 'st-rework';
  if (decision.startsWith('MRB'))                      return 'st-mrb';
  if (decision === 'SCRAP')                            return 'st-scrap';
  if (decision === 'VOID')                             return 'st-void';
  return 'st-pending';
}

// ── Helpers ───────────────────────────────────────────────────────────────────
function photoUrl(id) { return `/api/photos/${id}/file`; }
function fmtMM(v)     { return (v == null) ? '—' : Number(v).toFixed(2) + ' mm'; }
function fmtVal(v)    { return (v == null || v === '') ? '—' : String(v); }
function fmtDate(s)   {
  try { return new Date(s).toLocaleDateString('tr-TR', { day:'2-digit', month:'2-digit', year:'numeric' }); }
  catch { return s || '—'; }
}
function decisionLabel(d)  { return DECISION_LABELS[d] || d || '—'; }
function statusBadge(st)   {
  const cls = st === 'completed' ? 'badge-completed' : st === 'rejected' ? 'badge-rejected' : 'badge-open';
  return `<span class="badge ${cls}">${STATUS_LABELS[st] || st}</span>`;
}

// ── Page header (portrait) ────────────────────────────────────────────────────
function portraitHeader(inspectionId, pageLabel) {
  return `
    <div class="page-header">
      <div class="hdr-left">
        <div class="hdr-logo-box">&#9741;</div>
        <div>
          <div class="hdr-brand-name">QualiSight</div>
          <div class="hdr-brand-sub">Inspection Management System</div>
        </div>
      </div>
      <div class="hdr-right">
        <div class="hdr-doc-title">Inspection Report</div>
        <div class="hdr-doc-number">Inspection #${inspectionId} &nbsp;·&nbsp; ${pageLabel}</div>
      </div>
    </div>`;
}

// ── Page header (landscape) ───────────────────────────────────────────────────
function landscapeHeader(data) {
  const items = [
    ['Part No',      fmtVal(data.part_number)],
    ['Serial No',    fmtVal(data.serial_number)],
    ['Operation No', fmtVal(data.operation_number)],
    ['Date',         fmtDate(data.created_at)],
  ].map(([l, v]) => `
    <div class="hdr-item">
      <div class="hi-label">${l}</div>
      <div class="hi-val">${v}</div>
    </div>`).join('');

  return `
    <div class="page-header">
      <div class="hdr-left">
        <div class="hdr-logo-box">&#9741;</div>
        <div>
          <div class="hdr-brand-name">QualiSight</div>
          <div class="hdr-brand-sub">Inspection #${data.id}</div>
        </div>
      </div>
      <div class="hdr-part-info">${items}</div>
      <div class="hdr-right">${fmtDate(data.created_at)}</div>
    </div>`;
}

// ── Pareto SVG ────────────────────────────────────────────────────────────────
function buildParetoSvg(byType) {
  const sorted = Object.entries(byType || {}).sort((a, b) => b[1] - a[1]);
  if (!sorted.length) return '<p style="color:#94a3b8;font-size:9pt;">No defect data available.</p>';

  const total = sorted.reduce((s, [, v]) => s + v, 0);
  const W = 480, H = 200;
  const PAD = { t: 16, r: 50, b: 42, l: 38 };
  const cW  = W - PAD.l - PAD.r;
  const cH  = H - PAD.t - PAD.b;
  const maxCount = sorted[0][1];
  const barW = cW / sorted.length;

  let bars = '', linePoints = '', xLabels = '', cumPct = 0;

  sorted.forEach(([name, count], i) => {
    const bh = (count / maxCount) * cH;
    const bx = PAD.l + i * barW;
    const by = PAD.t + cH - bh;
    bars += `<rect x="${(bx+2).toFixed(1)}" y="${by.toFixed(1)}" width="${(barW-4).toFixed(1)}" height="${bh.toFixed(1)}" fill="#1a3a5c" opacity="0.75" rx="2"/>`;
    bars += `<text x="${(bx+barW/2).toFixed(1)}" y="${(by-4).toFixed(1)}" text-anchor="middle" font-size="9" fill="#475569">${count}</text>`;

    cumPct += (count / total) * 100;
    const ly = PAD.t + cH - (cumPct / 100) * cH;
    const lx = PAD.l + i * barW + barW / 2;
    linePoints += `${lx.toFixed(1)},${ly.toFixed(1)} `;

    const label = name.length > 12 ? name.substring(0, 11) + '…' : name;
    xLabels += `<text x="${(bx+barW/2).toFixed(1)}" y="${(H-PAD.b+14).toFixed(1)}" text-anchor="middle" font-size="8.5" fill="#64748b">${label}</text>`;

    if (i === 0) {
      [0, 25, 50, 75, 100].forEach(pct => {
        const ty = PAD.t + cH - (pct / 100) * cH;
        bars += `<line x1="${PAD.l-3}" y1="${ty.toFixed(1)}" x2="${W-PAD.r}" y2="${ty.toFixed(1)}" stroke="#e2e8f0" stroke-width="0.7"/>`;
        bars += `<text x="${(W-PAD.r+4).toFixed(1)}" y="${(ty+3).toFixed(1)}" font-size="8" fill="#94a3b8">${pct}%</text>`;
      });
    }
  });

  return `
    <svg class="pareto-svg" viewBox="0 0 ${W} ${H}">
      <rect x="${PAD.l}" y="${PAD.t}" width="${cW}" height="${cH}" fill="#f8fafc" rx="2"/>
      ${bars}
      <polyline points="${linePoints.trim()}" fill="none" stroke="#ef4444" stroke-width="2" stroke-linejoin="round"/>
      ${xLabels}
      <line x1="${PAD.l}" y1="${PAD.t}" x2="${PAD.l}" y2="${PAD.t+cH}" stroke="#cbd5e1" stroke-width="1"/>
      <line x1="${PAD.l}" y1="${PAD.t+cH}" x2="${W-PAD.r}" y2="${PAD.t+cH}" stroke="#cbd5e1" stroke-width="1"/>
    </svg>`;
}

// ── Measurement grid (portrait — 6-col) ──────────────────────────────────────
function measGrid(d) {
  const cells = [
    ['Depth',  fmtMM(d.depth)],
    ['Width',  fmtMM(d.width)],
    ['Length', fmtMM(d.length)],
    ['Radius', fmtMM(d.radius)],
    ['Angle',  d.angle  != null ? d.angle + '°' : '—'],
    ['Color',  fmtVal(d.color)],
  ].map(([l, v]) => `
    <div class="meas-cell">
      <div class="mc-label">${l}</div>
      <div class="mc-val">${v}</div>
    </div>`).join('');
  return `<div class="meas-grid">${cells}</div>`;
}

// ── Measurement snapshot parser ───────────────────────────────────────────────
function parseMeas(snapshot) {
  if (!snapshot) return null;
  try { return JSON.parse(snapshot); } catch { return null; }
}

function measSnapshotCells(meas) {
  const LABELS = { depth:'Depth', width:'Width', length:'Length', radius:'Radius', angle:'Angle', color:'Color' };
  return Object.entries(LABELS)
    .filter(([k]) => meas[k] != null)
    .map(([k, l]) => `<span class="snap-cell"><span class="snap-lbl">${l}</span><span class="snap-val">${k==='angle' ? meas[k]+'°' : k==='color' ? meas[k] : Number(meas[k]).toFixed(2)+' mm'}</span></span>`)
    .join('');
}

// Pre/Post comparison for RE_INSPECT — preMeas=snapshot at RE_INSPECT, postMeas=next snapshot or current defect
function prePostTable(preMeas, postMeas) {
  if (!preMeas) return '';
  const KEYS = ['depth','width','length','radius','angle','color'];
  const LABELS = { depth:'Depth', width:'Width', length:'Length', radius:'Radius', angle:'Angle', color:'Color' };
  const rows = KEYS
    .filter(k => preMeas[k] != null || (postMeas && postMeas[k] != null))
    .map(k => {
      const pre  = preMeas[k]  != null ? (k==='angle' ? preMeas[k]+'°'  : k==='color' ? preMeas[k]  : Number(preMeas[k]).toFixed(2)+' mm')  : '—';
      const post = postMeas && postMeas[k] != null ? (k==='angle' ? postMeas[k]+'°' : k==='color' ? postMeas[k] : Number(postMeas[k]).toFixed(2)+' mm') : '—';
      const changed = pre !== post && post !== '—';
      return `<tr${changed?' class="meas-changed"':''}>
        <td class="ppt-lbl">${LABELS[k]}</td>
        <td class="ppt-pre">${pre}</td>
        <td class="ppt-arrow">→</td>
        <td class="ppt-post${changed?' changed':''}">${post}</td>
      </tr>`;
    }).join('');
  if (!rows) return '';
  return `<div class="pre-post-wrap">
    <div class="pre-post-title">Measurement Changes (Re-Inspection)</div>
    <table class="pre-post-table">
      <thead><tr><th></th><th>Before</th><th></th><th>After</th></tr></thead>
      <tbody>${rows}</tbody>
    </table>
  </div>`;
}

// ── Defect link notice ────────────────────────────────────────────────────────
function defectLinks(d) {
  let html = '';
  if (d.origin_defect_id) {
    html += `<div class="defect-link-notice origin-notice">&#9654; This defect was created as a result of rework/re-inspection of <strong>Defect #${d.origin_defect_id}</strong>.</div>`;
  }
  if (d.child_defect_ids && d.child_defect_ids.length > 0) {
    const ids = d.child_defect_ids.map(i => `<strong>#${i}</strong>`).join(', ');
    html += `<div class="defect-link-notice child-notice">&#9654; Derived defect(s) from this defect: ${ids}</div>`;
  }
  return html;
}

// ── Disposition pills (portrait) ──────────────────────────────────────────────
function dispRows(dispositions, defect) {
  if (!dispositions.length)
    return '<p style="font-size:8.5pt;color:#94a3b8;padding:4px 0;">No disposition entered.</p>';
  return dispositions.map((dp, idx) => {
    const snap = parseMeas(dp.measurements_snapshot);
    let extra = '';
    if (dp.decision === 'RE_INSPECT' && snap) {
      // post = next disposition's snapshot, or current defect values
      const nextDp    = dispositions[idx + 1];
      const postMeas  = nextDp ? parseMeas(nextDp.measurements_snapshot) : {
        depth: defect?.depth, width: defect?.width, length: defect?.length,
        radius: defect?.radius, angle: defect?.angle, color: defect?.color,
      };
      extra = prePostTable(snap, postMeas);
    } else if (snap && dp.decision !== 'RE_INSPECT') {
      const cells = measSnapshotCells(snap);
      if (cells) extra = `<div class="snap-row">${cells}</div>`;
    }
    return `<div class="disp-row">
      <span class="disp-pill dp-${dp.decision}">${decisionLabel(dp.decision)}</span>
      <span class="disp-note">${dp.note || ''}</span>
      <span class="disp-ts">${fmtDate(dp.decided_at)}${dp.entered_by ? ' — ' + dp.entered_by : ''}</span>
    </div>${extra}`;
  }).join('');
}

// ── Photo grid (portrait) ─────────────────────────────────────────────────────
function photoGrid(photos) {
  if (!photos.length) return `
    <div class="photo-placeholder">
      <div class="ph-icon">&#128247;</div>
      <div>No photos added</div>
    </div>`;
  const countCls = photos.length === 1 ? 'photo-count-1'
    : photos.length === 2 ? 'photo-count-2'
    : photos.length === 3 ? 'photo-count-3'
    : 'photo-count-many';
  const imgs = photos.map((p, i) => `
    <div>
      <img src="${photoUrl(p.id)}" loading="lazy" alt="Photo ${i+1}"/>
      <div class="photo-caption">Photo ${i+1} / ${photos.length} &nbsp;·&nbsp; #${p.id}</div>
    </div>`).join('');
  return `<div class="dc-photo ${countCls}">${imgs}</div>`;
}

// ── PORTRAIT REPORT ───────────────────────────────────────────────────────────
function renderPortrait(data) {
  const root = document.getElementById('report-root');
  root.classList.add('portrait');
  document.title = `Report — Inspection #${data.id}`;
  document.getElementById('toolbar-title').textContent = `Inspection #${data.id} — A4 Portrait Report`;

  const totalPages = 1 + data.defects.length;
  const s = data.summary;
  let pages = '';

  // ── Cover page ────────────────────────────────────────────────────────────
  pages += `
    <div class="page">
      ${portraitHeader(data.id, 'Page 1 / ' + totalPages)}

      <div class="section-title">Inspection Details</div>
      <table class="info-table">
        <tr>
          <th>Part No</th>       <td>${fmtVal(data.part_number)}</td>
          <th>Serial No</th>     <td>${fmtVal(data.serial_number)}</td>
        </tr>
        <tr>
          <th>Operation No</th>  <td>${fmtVal(data.operation_number)}</td>
          <th>Date</th>          <td>${fmtDate(data.created_at)}</td>
        </tr>
      </table>

      <div class="section-title">Summary</div>
      <div class="summary-row">
        <div class="sum-box">
          <div class="val">${s.total}</div>
          <div class="lbl">Total Defects</div>
        </div>
        <div class="sum-box ok">
          <div class="val">${s.neutralized}</div>
          <div class="lbl">Resolved</div>
        </div>
        <div class="sum-box warn">
          <div class="val">${s.pending}</div>
          <div class="lbl">Pending</div>
        </div>
        <div class="sum-box">
          <div class="val">${data.defects.filter(d => d.photos.length > 0).length}</div>
          <div class="lbl">Defects with Photos</div>
        </div>
      </div>

      <div class="section-title">Defect Distribution by Type (Pareto)</div>
      <div class="pareto-wrap">${buildParetoSvg(s.by_type)}</div>

      ${data.notes ? `
        <div class="section-title">Notes</div>
        <div class="notes-box">${data.notes}</div>
      ` : ''}

      <div class="page-footer">
        <span>Inspection #${data.id} &nbsp;·&nbsp; ${fmtVal(data.part_number)} / ${fmtVal(data.serial_number)}</span>
        <span class="footer-confidential">Confidential</span>
        <span>Page 1 / ${totalPages}</span>
      </div>
    </div>`;

  // ── Defect pages ──────────────────────────────────────────────────────────
  data.defects.forEach((d, idx) => {
    const pageNum    = idx + 2;
    const activeDec  = d.active_disposition?.decision || null;
    const activeLabel = decisionLabel(activeDec || 'PENDING');
    const dpClass    = activeDec ? `dp-${activeDec}` : 'dp-PENDING';
    const hmBadge    = d.high_metal ? '<span class="dc-hm-badge">High Metal</span>' : '';

    pages += `
      <div class="page">
        ${portraitHeader(data.id, 'Defect #' + d.id)}

        <div class="defect-card">
          <div class="dc-header">
            <span class="dc-id">#${d.id}</span>
            <span class="dc-type">${fmtVal(d.defect_type_name)}</span>
            ${hmBadge}
            <span class="dc-disp-badge ${dpClass}">${activeLabel}</span>
          </div>

          ${defectLinks(d)}

          <div class="dc-measurements">
            ${measGrid(d)}
            ${d.notes ? `<div class="meas-notes"><span>Note:</span> ${d.notes}</div>` : ''}
          </div>

          ${photoGrid(d.photos)}

          <div class="dc-disposition">
            <div class="disp-section-label">Disposition History</div>
            ${dispRows(d.dispositions, d)}
          </div>
        </div>

        <div class="page-footer">
          <span>Defect #${d.id} &nbsp;·&nbsp; ${fmtVal(d.defect_type_name)}</span>
          <span class="footer-confidential">Confidential</span>
          <span>Page ${pageNum} / ${totalPages}</span>
        </div>
      </div>`;
  });

  root.innerHTML = pages;
}

// ── LANDSCAPE REPORT ──────────────────────────────────────────────────────────
function renderLandscape(data) {
  // Override @page to landscape
  const pageStyle = document.createElement('style');
  pageStyle.textContent = '@page { size: A4 landscape; margin: 0; }';
  document.head.appendChild(pageStyle);

  const root = document.getElementById('report-root');
  root.classList.add('landscape');
  document.title = `Report — Inspection #${data.id}`;
  document.getElementById('toolbar-title').textContent = `Inspection #${data.id} — A4 Landscape Report`;

  let totalPages = 0;
  data.defects.forEach(d => { totalPages += Math.max(1, d.photos.length); });

  const hdr = landscapeHeader(data);
  let pages = '';
  let pageNum = 0;

  data.defects.forEach(d => {
    const activeDec   = d.active_disposition?.decision || null;
    const activeLabel = decisionLabel(activeDec || 'PENDING');
    const statusCls   = defStatusClass(activeDec);
    const hmBadge     = d.high_metal ? '<span class="def-hm">High Metal</span>' : '';
    const photoCount  = d.photos.length;

    const defectBar = `
      <div class="defect-bar">
        <span class="def-id">#${d.id}</span>
        <span class="def-type">${fmtVal(d.defect_type_name)}</span>
        ${hmBadge}
        <span class="def-status ${statusCls}">${activeLabel}</span>
      </div>`;

    // Measurements (3-col landscape grid)
    const measCells = [
      ['Depth',  fmtMM(d.depth)],
      ['Width',  fmtMM(d.width)],
      ['Length', fmtMM(d.length)],
      ['Radius', fmtMM(d.radius)],
      ['Angle',  d.angle != null ? d.angle + '°' : '—'],
      ['Color',  fmtVal(d.color)],
    ].map(([l, v]) => `
      <div class="ls-meas-cell">
        <div class="ls-mc-label">${l}</div>
        <div class="ls-mc-val">${v}</div>
      </div>`).join('');

    // Disposition table
    const dispTableRows = d.dispositions.length
      ? d.dispositions.map((dp, idx) => {
          const isActive = d.active_disposition && dp.id === d.active_disposition.id;
          const snap = parseMeas(dp.measurements_snapshot);
          let snapHtml = '';
          if (dp.decision === 'RE_INSPECT' && snap) {
            const nextDp   = d.dispositions[idx + 1];
            const postMeas = nextDp ? parseMeas(nextDp.measurements_snapshot) : {
              depth: d.depth, width: d.width, length: d.length,
              radius: d.radius, angle: d.angle, color: d.color,
            };
            snapHtml = `<tr><td colspan="3" style="padding:0 4px 4px;">${prePostTable(snap, postMeas)}</td></tr>`;
          } else if (snap) {
            const cells = measSnapshotCells(snap);
            if (cells) snapHtml = `<tr><td colspan="3"><div class="snap-row" style="padding:2px 0;">${cells}</div></td></tr>`;
          }
          return `<tr${isActive ? ' class="disp-active-row"' : ''}>
            <td><span class="disp-pill dp-${dp.decision}">${decisionLabel(dp.decision)}</span></td>
            <td class="disp-note-td">${dp.note || ''}</td>
            <td class="disp-ts-td">${fmtDate(dp.decided_at)}${dp.entered_by ? '<br/>' + dp.entered_by : ''}</td>
          </tr>${snapHtml}`;
        }).join('')
      : `<tr><td colspan="3" style="color:#94a3b8;font-size:7.5pt;">No disposition entered.</td></tr>`;

    // ── First page (58% photo + 42% info) ─────────────────────────────────
    pageNum++;
    const photo0 = d.photos[0] || null;
    const photoPane0 = photo0
      ? `<img src="${photoUrl(photo0.id)}" loading="lazy" alt="Photo 1"/>
         ${photoCount > 1 ? `<div class="photo-id-stamp">#${d.id} &nbsp;·&nbsp; 1 / ${photoCount}</div>` : ''}`
      : `<div class="photo-placeholder">
           <div class="ph-icon">&#128247;</div>
           <div>No photos added</div>
         </div>`;

    pages += `
      <div class="page">
        ${hdr}
        ${defectBar}
        <div class="ls-page-body">
          <div class="photo-pane">${photoPane0}</div>
          <div class="info-pane">
            ${defectLinks(d)}
            <div class="ls-section-label">Measurements</div>
            <div class="ls-meas-grid">${measCells}</div>
            ${d.notes ? `<div class="ls-notes-line"><span>Note:</span> ${d.notes}</div>` : ''}
            <div class="ls-section-label">Disposition</div>
            <table class="disp-table"><tbody>${dispTableRows}</tbody></table>
            ${photoCount > 1 ? `<div class="ls-photo-count-note">&#43; ${photoCount - 1} more photo(s) — continued on next pages</div>` : ''}
          </div>
        </div>
        <div class="ls-page-footer">
          <span>Defect #${d.id} &nbsp;·&nbsp; ${fmtVal(d.defect_type_name)}</span>
          <span class="footer-confidential">Confidential</span>
          <span>Page ${pageNum} / ${totalPages}</span>
        </div>
      </div>`;

    // ── Continuation pages (85% photo + 15% strip) ────────────────────────
    for (let pi = 1; pi < photoCount; pi++) {
      pageNum++;
      const photo = d.photos[pi];
      pages += `
        <div class="page">
          ${hdr}
          ${defectBar}
          <div class="ls-page-body">
            <div class="photo-pane-wide">
              <img src="${photoUrl(photo.id)}" loading="lazy" alt="Photo ${pi+1}"/>
              <div class="photo-id-stamp">#${d.id} &nbsp;·&nbsp; ${pi+1} / ${photoCount}</div>
            </div>
            <div class="info-strip">
              <div class="is-id">#${d.id}</div>
              <div class="is-type">${fmtVal(d.defect_type_name)}</div>
              <div class="is-counter">
                <strong>${pi+1} / ${photoCount}</strong>
                photos
              </div>
            </div>
          </div>
          <div class="ls-page-footer">
            <span>Defect #${d.id} &nbsp;·&nbsp; ${fmtVal(d.defect_type_name)}</span>
            <span class="footer-confidential">Confidential</span>
            <span>Page ${pageNum} / ${totalPages}</span>
          </div>
        </div>`;
    }
  });

  if (!data.defects.length) {
    pages = `
      <div class="page">
        ${hdr}
        <div class="ls-page-body" style="align-items:center;justify-content:center;flex:1;">
          <p style="color:#94a3b8;font-size:12pt;">No defect records found for this inspection.</p>
        </div>
      </div>`;
  }

  root.innerHTML = pages;
}

// ── Main ──────────────────────────────────────────────────────────────────────
async function main() {
  const params = new URLSearchParams(location.search);
  const id     = params.get('id');
  const type   = params.get('type') || 'portrait';
  const root   = document.getElementById('report-root');

  if (!id) {
    root.innerHTML = '<div class="error-screen">Error: Inspection ID not specified (?id=X)</div>';
    return;
  }

  try {
    const res = await fetch(`/api/inspections/${id}/report-data`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const data = await res.json();

    if (type === 'landscape') {
      renderLandscape(data);
    } else {
      renderPortrait(data);
    }

    if (params.get('print') === '1') {
      setTimeout(() => window.print(), 800);
    }
  } catch (err) {
    root.innerHTML = `<div class="error-screen">Rapor yüklenemedi: ${err.message}</div>`;
  }
}

main();

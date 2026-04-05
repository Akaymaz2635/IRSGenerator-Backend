import { api }        from '../api.js';
import { Camera }     from '../camera.js';
import { Annotator }  from '../annotator.js';
import { session }    from '../session.js';

// ── Disposition helpers (API-driven, cached) ──────────────────────────────────
let _dispTypeCache = null;           // DispositionType[]
let _dispTransCache = null;          // { [fromCode|'__null__']: string[] }

async function _loadDispTypes() {
  if (_dispTypeCache) return _dispTypeCache;
  try { _dispTypeCache = await api.dispositionTypes.list(); } catch { _dispTypeCache = []; }
  return _dispTypeCache;
}

async function _loadDispTrans() {
  if (_dispTransCache) return _dispTransCache;
  try {
    const rows = await api.dispositionTransitions.list();
    _dispTransCache = {};
    for (const row of rows) {
      const key = row.from_code ?? '__null__';
      if (!_dispTransCache[key]) _dispTransCache[key] = [];
      _dispTransCache[key].push(row.to_code);
    }
  } catch { _dispTransCache = {}; }
  return _dispTransCache;
}

// Returns { label, cls } for a decision code, with safe fallback
async function getDecisionMeta(code) {
  const types = await _loadDispTypes();
  const found = types.find(t => t.code === code);
  return found
    ? { label: found.label, cls: found.css_class }
    : { label: code, cls: 'disp-unknown' };
}

// Returns true if this decision neutralizes (closes) the defect
async function isNeutralizing(code) {
  const types = await _loadDispTypes();
  return types.some(t => t.code === code && t.is_neutralizing);
}

// Returns string[] of allowed next decision codes for a given current decision
async function allowedNextDecisions(currentDecision) {
  const trans = await _loadDispTrans();
  const key = currentDecision ? currentDecision : '__null__';
  return trans[key] ?? [];
}

// ── Dinamik alan sabitleri ────────────────────────────────────────────────────
const DEFAULT_FIELDS = [
  { field_name: 'depth',  label: 'Derinlik', unit: 'mm', field_type: 'number' },
  { field_name: 'width',  label: 'Genişlik', unit: 'mm', field_type: 'number' },
  { field_name: 'length', label: 'Uzunluk',  unit: 'mm', field_type: 'number' },
  { field_name: 'radius', label: 'Yarıçap',  unit: 'mm', field_type: 'number' },
  { field_name: 'angle',  label: 'Açı',      unit: '°',  field_type: 'number' },
  { field_name: 'color',  label: 'Renk',     unit: '',   field_type: 'text'   },
];

// Cache: defect_type_id → fields[]
const _fieldsCache = {};
async function getDefectFields(defect_type_id) {
  if (!defect_type_id) return DEFAULT_FIELDS;
  if (_fieldsCache[defect_type_id] !== undefined) return _fieldsCache[defect_type_id];
  try {
    const fields = await api.defectFields.list(defect_type_id);
    _fieldsCache[defect_type_id] = fields && fields.length ? fields : DEFAULT_FIELDS;
  } catch {
    _fieldsCache[defect_type_id] = DEFAULT_FIELDS;
  }
  return _fieldsCache[defect_type_id];
}

// ── Pre/Post rework ölçüm karşılaştırma tablosu ───────────────────────────
function renderMeasurementComparison(defect, fields) {
  const usedFields = fields && fields.length ? fields : DEFAULT_FIELDS;

  const reworkDisp = [...(defect.dispositions || [])]
    .reverse()
    .find(d => d.decision === 'REWORK' && d.measurements_snapshot);
  if (!reworkDisp) return '';

  let pre;
  try { pre = JSON.parse(reworkDisp.measurements_snapshot); } catch { return ''; }

  const rows = usedFields.map((f) => {
    const key   = f.field_name;
    const label = f.label;
    const unit  = f.unit || '';
    const preVal  = pre[key];
    const postVal = defect[key];
    if (preVal == null && postVal == null) return '';

    const fmtVal = (v) => v == null ? '—'
      : unit === 'mm' ? Number(v).toFixed(2) + ' mm'
      : unit === '°'  ? v + '°'
      : String(v);

    let diffHtml = '';
    if (unit === 'mm' && preVal != null && postVal != null) {
      const d = postVal - preVal;
      const cls = d < 0 ? 'diff-down' : d > 0 ? 'diff-up' : 'diff-none';
      diffHtml = `<span class="${cls}">${d < 0 ? '▼' : d > 0 ? '▲' : '='} ${Math.abs(d).toFixed(2)} mm</span>`;
    } else if (preVal !== postVal) {
      diffHtml = '<span class="diff-changed">Değişti</span>';
    } else {
      diffHtml = '<span class="diff-none">—</span>';
    }

    return `<tr><td>${label}</td><td>${fmtVal(preVal)}</td><td>${fmtVal(postVal)}</td><td>${diffHtml}</td></tr>`;
  }).filter(Boolean).join('');

  if (!rows) return '';
  return `
    <div class="rework-comparison">
      <div class="rework-comparison-title">📊 Pre / Post Rework Ölçüm Karşılaştırması</div>
      <table class="comparison-table">
        <thead><tr><th>Ölçüm</th><th>Pre-Rework</th><th>Post-Rework</th><th>Fark</th></tr></thead>
        <tbody>${rows}</tbody>
      </table>
    </div>`;
}

function dispositionBadge(disp) {
  if (!disp) return '<span class="disp-badge disp-pending">Bekliyor</span>';
  const found = (_dispTypeCache || []).find(t => t.code === disp.decision);
  const label = found ? found.label : disp.decision;
  const cls   = found ? found.css_class : '';
  return `<span class="disp-badge ${cls}" title="${disp.note}">${label}</span>`;
}

// ── Helpers ───────────────────────────────────────────────────────────────────
function formatLimits(lower, upper) {
  const INF = 1.7976931348623157e+308;
  if (lower != null && upper != null && upper >= INF) return `≥ ${lower.toFixed(4)}`;
  if (upper != null && lower != null && lower <= 0 && upper < INF) return `≤ ${upper.toFixed(4)}`;
  if (lower != null && upper != null) return `${lower.toFixed(4)} ↔ ${upper.toFixed(4)}`;
  return 'No limits defined';
}

function statusBadge(status) {
  const map    = { open: 'badge-open', completed: 'badge-completed', rejected: 'badge-rejected' };
  const labels = { open: 'Open', completed: 'Completed', rejected: 'Rejected' };
  return `<span class="badge ${map[status] || ''}">${labels[status] || status}</span>`;
}
function fmt(v)         { return (v === null || v === undefined) ? '—' : String(v); }
function fmtMM(v)       { return (v === null || v === undefined) ? '—' : Number(v).toFixed(2) + ' mm'; }
function fmtDate(s)     { try { return new Date(s).toLocaleString('tr-TR'); } catch { return s || '—'; } }

// Defect checkboxes — many-to-many selection
function defectCheckboxes(defects, selectedIds = []) {
  if (!defects.length) return '<p class="text-secondary" style="font-size:13px;">Henüz hata kaydı yok.</p>';
  const sel = new Set(selectedIds.map(Number));
  return `<div class="defect-checklist">
    ${defects.map(d => `
      <label class="defect-check-item">
        <input type="checkbox" name="defect_ids" value="${d.id}"
               ${sel.has(d.id) ? 'checked' : ''} />
        <span>#${d.id} · ${d.defect_type_name || 'Hata'}</span>
      </label>`).join('')}
  </div>`;
}

// Collect checked defect IDs from a container element by id
function getCheckedDefectIds(containerId) {
  return Array.from(
    document.querySelectorAll(`#${containerId} input[name="defect_ids"]:checked`)
  ).map(el => Number(el.value));
}

// ── Ana sayfa ────────────────────────────────────────────────────────────────
export async function inspectionDetailPage(id) {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Loading...</div>';
  await render(id, root);
}

async function render(id, root) {
  // Pre-load disposition type + transition caches so badge/isNeutralized run synchronously
  await Promise.all([_loadDispTypes(), _loadDispTrans()]);

  const [detail, defectTypes, allPhotos] = await Promise.all([
    api.inspections.get(id),
    api.defectTypes.list(),
    api.photos.list({ inspection_id: id }),
  ]);

  if (!detail) {
    root.innerHTML = '<div class="empty text-danger">Inspection not found.</div>';
    return;
  }

  const dtMap = Object.fromEntries(defectTypes.map(d => [d.id, d]));

  // Pre-fetch defect fields for all unique defect_type_ids
  const uniqueTypeIds = [...new Set(detail.defects.map(d => d.defect_type_id).filter(Boolean))];
  await Promise.all(uniqueTypeIds.map(tid => getDefectFields(tid)));
  // fieldsMap: defect_type_id → fields[]
  const fieldsMap = Object.fromEntries(uniqueTypeIds.map(tid => [tid, _fieldsCache[tid] || DEFAULT_FIELDS]));

  // Many-to-many grouping: a photo can appear in multiple defect buckets
  const photoByDefect = {};
  for (const p of allPhotos) {
    if (!p.defect_ids || p.defect_ids.length === 0) {
      (photoByDefect['general'] ??= []).push(p);
    } else {
      for (const did of p.defect_ids) {
        (photoByDefect[did] ??= []).push(p);
      }
    }
  }
  const generalPhotos = photoByDefect['general'] || [];

  // ── Hata satırları ──────────────────────────────────────────
  const defectRows = detail.defects.map((d) => {
    const dPhotos  = photoByDefect[d.id] || [];
    const decision = d.active_disposition?.decision || null;
    const isNeutralized = (_dispTypeCache || []).some(t => t.code === decision && t.is_neutralizing);

    const thumbs = dPhotos.slice(0, 4).map(p =>
      `<div class="thumb-wrap">
         <img class="thumb" src="${api.photos.fileUrl(p.id)}" title="Fotoğraf #${p.id}"
              onclick="window.open('${api.photos.fileUrl(p.id)}','_blank')" />
         <div class="thumb-btns">
           <button class="thumb-ann-btn" data-pid="${p.id}"
                   data-defects='${JSON.stringify(p.defect_ids || [])}' title="Düzenle">✏</button>
           <button class="thumb-del-btn" data-pid="${p.id}" title="Sil">✕</button>
         </div>
       </div>`
    ).join('');
    const more = dPhotos.length > 4
      ? `<span class="thumb-more">+${dPhotos.length - 4}</span>` : '';

    // Action buttons based on disposition state
    const canWrite = session.canWrite();
    let dispActions = '';
    if (canWrite) {
      if (!isNeutralized) {
        const label = decision === 'REWORK' ? '↩ Yeniden İncele' : '⚖ Karar';
        dispActions = `<button class="btn btn-ghost btn-xs disp-btn" data-id="${d.id}"
                title="Disposition gir / güncelle">${label}</button>`;
        // REWORK: yeniden işleme sonrası re-inspect implicit — yeni hata eklenebilir
        if (decision === 'REWORK') {
          dispActions += ` <button class="btn btn-primary btn-xs rework-child-btn" data-id="${d.id}"
                  title="Rework sonrası tespit edilen yeni hata ekle">+ Yeniden İşleme Hatası</button>`;
        }
        // CTP_RE_INSPECT: sonraki operasyonda tespit edilen hata
        if (decision === 'CTP_RE_INSPECT') {
          dispActions += ` <button class="btn btn-primary btn-xs rework-child-btn" data-id="${d.id}"
                  title="Sonraki operasyonda tespit edilen yeni hata ekle">+ CTP Hatası</button>`;
        }
      }
    }

    // Sub-row for lineage info
    const subParts = [];
    if (d.origin_defect_id) {
      subParts.push(`<span class="origin-label">↳ Yeniden işleme sonucu (Hata #${d.origin_defect_id})</span>`);
    }
    if (d.child_defect_ids?.length > 0) {
      const ids = d.child_defect_ids.map(cid => `#${cid}`).join(', ');
      subParts.push(`<span class="child-label">⚠ Yeniden işleme: ${d.child_defect_ids.length} yeni hata (${ids})</span>`);
    }
    const comparison = renderMeasurementComparison(d, fieldsMap[d.defect_type_id]);
    const subContent = [
      subParts.length ? subParts.join(' &nbsp;·&nbsp; ') : '',
      comparison,
    ].filter(Boolean).join('');
    const subRow = subContent
      ? `<tr class="defect-sub-row"><td colspan="11">${subContent}</td></tr>`
      : '';

    return `<tr${d.high_metal ? ' class="row-high-metal"' : ''}>
      <td class="col-id">#${d.id}</td>
      <td>
        ${d.defect_type_name || dtMap[d.defect_type_id]?.name || '—'}
        ${d.high_metal ? '<br><span style="font-size:11px;color:#b35900;font-weight:600;">⬆ High Metal</span>' : ''}
      </td>
      <td>${fmtMM(d.depth)}</td>
      <td>${fmtMM(d.width)}</td>
      <td>${fmtMM(d.length)}</td>
      <td>${fmtMM(d.radius)}</td>
      <td>${d.angle != null ? d.angle + '°' : '—'}</td>
      <td>${d.color || '—'}</td>
      <td class="thumb-cell">${thumbs}${more}
        <button class="btn btn-ghost btn-xs add-photo-to-defect"
                data-defect-id="${d.id}"
                title="Bu hataya fotoğraf ekle">+📷</button>
      </td>
      <td>
        ${dispositionBadge(d.active_disposition)}
        ${canWrite && d.active_disposition ? `<button class="btn btn-ghost btn-xs disp-undo-btn" data-disp-id="${d.active_disposition.id}" title="Bu kararı geri al ve yeniden gir">✕ Düzelt</button>` : ''}
        ${dispActions}
      </td>
      <td class="col-actions">
        ${canWrite ? `<button class="btn btn-ghost btn-xs edit-defect-btn" data-id="${d.id}">Düzenle</button>` : ''}
        ${canWrite ? `<button class="btn btn-danger btn-xs delete-defect-btn" data-id="${d.id}">Sil</button>` : ''}
      </td>
    </tr>${subRow}`;
  }).join('');

  // ── Genel fotoğraf kartları ──────────────────────────────────
  function photoCard(p) {
    const defectBadges = (p.defect_ids || []).map(did =>
      `<span class="badge-defect">#${did}</span>`
    ).join('');
    return `<div class="photo-card">
      <img src="${api.photos.fileUrl(p.id)}" alt="Fotoğraf #${p.id}"
           loading="lazy"
           onclick="window.open('${api.photos.fileUrl(p.id)}','_blank')" />
      <div class="photo-info">
        <div>${defectBadges}</div>
        <span class="photo-info-date">${fmtDate(p.created_at).split(' ')[0]}</span>
        <div class="photo-actions">
          <button class="photo-annotate-btn" data-id="${p.id}"
                  data-defects='${JSON.stringify(p.defect_ids || [])}' title="Fotoğrafı Düzenle">✏</button>
          <button class="photo-delete-btn"   data-id="${p.id}" title="Sil">✕</button>
        </div>
      </div>
    </div>`;
  }

  const pageCanWrite = session.canWrite();

  // ── HTML ─────────────────────────────────────────────────────
  root.innerHTML = `
    <a href="#/inspections" class="back-link">← Inspections</a>

    <div class="page-header">
      <h1>Inspection #${id} ${statusBadge(detail.status)}</h1>
      <div class="page-header-actions">
        ${pageCanWrite ? `
        <select class="status-select" id="status-quick-change">
          <option value="open"      ${detail.status==='open'      ?'selected':''}>Open</option>
          <option value="completed" ${detail.status==='completed' ?'selected':''}>Completed</option>
          <option value="rejected"  ${detail.status==='rejected'  ?'selected':''}>Rejected</option>
        </select>` : ''}
        <div class="dropdown" id="report-dropdown">
          <button class="btn btn-ghost btn-sm" id="report-dropdown-btn">Visual Report ▾</button>
          <div class="dropdown-menu" id="report-dropdown-menu">
            <button class="dropdown-item" id="report-landscape-btn">A4 Yatay (Hata başına sayfa)</button>
            <button class="dropdown-item" id="report-detail-btn">📊 Detail Report</button>
          </div>
        </div>
        <a class="btn btn-ghost btn-sm" id="word-report-btn" href="${api.inspections.reportUrl(id)}" download>📄 Dimensional Report</a>
        <a class="btn btn-ghost btn-sm" href="${api.inspections.detailReportUrl(id)}" target="_blank">📋 Detailed Report</a>
        <a href="#/inspections/${id}/ncr" class="btn btn-ghost btn-sm">⚠ NC Descriptions</a>
        ${pageCanWrite ? `<a href="#/inspections/${id}/edit" class="btn btn-ghost btn-sm">Düzenle</a>` : ''}
        ${pageCanWrite ? `<button class="btn btn-danger btn-sm" id="delete-insp-btn">Sil</button>` : ''}
      </div>
    </div>

    <!-- Bilgi kartı -->
    <div class="card">
      <div class="card-header"><span class="card-title">Inspection Info</span></div>
      <div class="info-grid">
        ${[
          ['Project',         detail.project_name],
          ['Part Number',     detail.part_number],
          ['Serial Number',   detail.serial_number],
          ['Operation No',    detail.operation_number],
          ['Inspector',       detail.inspector],
          ...(detail.date ? [['Date', detail.date]] : []),
          ['Created',         fmtDate(detail.created_at)],
        ].map(([l, v]) => `
          <div class="info-item">
            <span class="info-label">${l}</span>
            <span class="info-value">${fmt(v)}</span>
          </div>`).join('')}
        ${detail.notes ? `
          <div class="info-item" style="grid-column:1/-1">
            <span class="info-label">Notes</span>
            <span class="info-value">${detail.notes}</span>
          </div>` : ''}
      </div>
    </div>

    <!-- Dual-Tab Bar -->
    <div class="dual-tab-bar">
      <button class="dual-tab active" data-tab="visual">🔍 Visual</button>
      <button class="dual-tab" data-tab="dimensional">📐 Dimensional</button>
    </div>

    <!-- Dimensional Tab -->
    <div id="tab-dimensional" class="tab-pane" style="display:none">
      <div id="dimensional-content"><div class="loading">Loading…</div></div>
    </div>

    <!-- Visual Tab wrapper start -->
    <div id="tab-visual" class="tab-pane">

    <!-- Hata kayıtları -->
    <div class="card">
      <div class="card-header">
        <span class="card-title">Defects (${detail.defects.length})</span>
        ${pageCanWrite ? '<button class="btn btn-primary btn-sm" id="add-defect-btn">+ Add Defect</button>' : ''}
      </div>
      <div class="table-wrapper">
        ${detail.defects.length === 0
          ? '<div class="empty">Kayıtlı hata bulunmuyor.</div>'
          : `<table class="data-table">
              <thead><tr>
                <th>ID</th><th>Defect Type</th><th>Depth</th><th>Width</th>
                <th>Length</th><th>Radius</th><th>Angle</th><th>Color</th>
                <th>Photos</th><th>Decision</th><th></th>
              </tr></thead>
              <tbody>${defectRows}</tbody>
            </table>`}
      </div>
    </div>

    <!-- Genel fotoğraflar -->
    <div class="card">
      <div class="card-header">
        <span class="card-title">
          Genel Fotoğraflar (${generalPhotos.length})
          <span class="text-secondary" style="font-size:12px;font-weight:400;">
            — Belirli bir hatayla ilişkilendirilmemiş
          </span>
        </span>
        ${pageCanWrite ? `<div class="d-flex gap-10 align-center">
          <button class="btn btn-primary btn-sm" id="open-camera-btn">📷 Kamera</button>
          <label class="btn btn-ghost btn-sm" for="photo-upload-input">📁 Dosya Yükle</label>
          <input type="file" id="photo-upload-input" class="file-input-hidden" accept="image/*" multiple />
        </div>` : ''}
      </div>
      ${generalPhotos.length === 0
        ? '<div class="empty">Genel fotoğraf bulunmuyor.</div>'
        : `<div class="photo-grid">${generalPhotos.map(photoCard).join('')}</div>`}
    </div>

    </div><!-- /tab-visual -->
  `;

  // ── Event listeners ───────────────────────────────────────────

  // ── Dual-tab switching ─────────────────────────────────────────
  // ── Dimensional tab state ─────────────────────────────────────
  let _dimLoaded    = false;
  let _chars        = [];
  let _activeIndex  = 0;
  let _activeDimModalKeyHandler = null;
  let _docHtml      = null;   // rendered mammoth HTML string
  let _docRowMap    = {};     // item_no (string) → <tr> element

  // char state map: id → { result: 'ok'|'fail'|null, actual: value }
  const _charState  = {};
  const _charDispState = {};   // character_id → active disposition object or null

  function _charBadgeHtml(c) {
    const s = _charState[c.id];
    const disp = _charDispState[c.id];
    if (!s || s.result === null) return '<span class="dim-char-dot dim-dot-pending">●</span>';
    if (s.result === 'ok') return '<span class="dim-char-dot dim-dot-ok">✓</span>';
    // fail — check disposition
    if (disp) {
      const isNeutral = (_dispTypeCache || []).some(t => t.code === disp.decision && t.is_neutralizing);
      if (isNeutral) return '<span class="dim-char-dot" style="color:var(--success);font-size:10px;font-weight:700" title="NC — Dispositioned (Neutral)">⚖✓</span>';
      return '<span class="dim-char-dot" style="color:#f59e0b;font-size:10px;font-weight:700" title="NC — Dispositioned (Pending)">⚖!</span>';
    }
    return '<span class="dim-char-dot dim-dot-fail">✗</span>';
  }

  function _isLot(c) {
    const b = (c.badge || '').toUpperCase();
    return b === 'LOT' || b === 'ATTRIBUTE';
  }

  function _renderStats() {
    const total   = _chars.length;
    const ok      = _chars.filter(c => _charState[c.id]?.result === 'ok').length;
    const fail    = _chars.filter(c => _charState[c.id]?.result === 'fail').length;
    const pending = total - ok - fail;
    const statsEl = root.querySelector('#dim-stats-row');
    if (statsEl) {
      statsEl.innerHTML = `
        <div class="irs-stat-card"><div class="irs-stat-value">${total}</div><div class="irs-stat-label">Total</div></div>
        <div class="irs-stat-card irs-stat-ok"><div class="irs-stat-value">${ok}</div><div class="irs-stat-label">Conform</div></div>
        <div class="irs-stat-card irs-stat-fail"><div class="irs-stat-value">${fail}</div><div class="irs-stat-label">Not Conform</div></div>
        <div class="irs-stat-card"><div class="irs-stat-value">${pending}</div><div class="irs-stat-label">Not Entered</div></div>`;
    }
  }

  function _renderProgress() {
    const entered  = _chars.filter(c => _charState[c.id]?.result !== null && _charState[c.id]?.result !== undefined).length;
    const pct      = _chars.length ? Math.round(entered / _chars.length * 100) : 0;
    const barEl    = root.querySelector('#dim-progress-bar');
    const lblEl    = root.querySelector('#dim-progress-label');
    if (barEl)  barEl.style.width = pct + '%';
    if (lblEl)  lblEl.textContent = `${entered}/${_chars.length}  (${pct}%)`;
  }

  function _updateCharListItem(index) {
    const c   = _chars[index];
    const li  = root.querySelector(`.dim-char-item[data-index="${index}"]`);
    if (!li) return;
    const dotEl = li.querySelector('.dim-char-dot-wrap');
    if (dotEl) dotEl.innerHTML = _charBadgeHtml(c);
  }

  function _highlightDocRow(c) {
    // Remove ONLY the active-highlight class (not the fail highlight)
    Object.values(_docRowMap).forEach(tr => tr.classList.remove('dim-doc-highlight'));
    const itemNoKey = String(c.item_no).replace(/\s+/g, '').toUpperCase();
    const tr = _docRowMap[itemNoKey];
    if (tr) {
      tr.classList.add('dim-doc-highlight');
      tr.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
    }
  }

  function _updateDocActualCell(c, actualValue, isOk) {
    const itemNoKey = String(c.item_no).replace(/\s+/g, '').toUpperCase();
    const tr = _docRowMap[itemNoKey];
    if (!tr) return;

    // Mark fail rows with permanent yellow highlight
    if (!isOk) tr.classList.add('dim-doc-fail');
    else        tr.classList.remove('dim-doc-fail');

    // Find ACTUAL column from the table's FIRST row (mammoth renders no <thead>)
    const table = tr.closest('table');
    if (!table) return;
    const allRows = table.querySelectorAll('tr');
    if (!allRows.length) return;
    const headerCells = Array.from(allRows[0].querySelectorAll('td, th'));
    let actualColIdx = headerCells.findIndex(th => /actual/i.test(th.textContent.trim()));
    if (actualColIdx < 0) actualColIdx = 2; // fallback: 3rd column is ACTUAL in standard op sheet

    const cells = tr.querySelectorAll('td, th');
    if (cells[actualColIdx]) {
      const cell = cells[actualColIdx];
      const rawStr = String(actualValue);
      const parts = rawStr.split('/').map(v => v.trim()).filter(Boolean);
      const hasLimitsC = c && (c.lower_limit !== 0 || c.upper_limit !== 0);
      if (parts.length > 1 && hasLimitsC) {
        cell.innerHTML = parts.map(p => {
          const n = parseFloat(p);
          const ok = !isNaN(n) && n >= c.lower_limit && n <= c.upper_limit;
          return `<span class="${ok ? 'val-ok' : 'val-fail'}">${p}</span>`;
        }).join('<span style="color:var(--text-secondary)"> / </span>');
      } else {
        cell.textContent = rawStr;
        cell.style.color      = isOk ? '#68d391' : '#fc8181';
        cell.style.fontWeight = '700';
      }
      cell.style.background = isOk ? 'rgba(56,161,105,0.35)' : 'rgba(229,62,62,0.35)';
    }
  }

  function _activateChar(index) {
    if (index < 0 || index >= _chars.length) return;
    _activeIndex = index;

    const c = _chars[index];

    // Highlight list item
    root.querySelectorAll('.dim-char-item').forEach(li => li.classList.remove('active'));
    const activeLi = root.querySelector(`.dim-char-item[data-index="${index}"]`);
    if (activeLi) {
      activeLi.classList.add('active');
      activeLi.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
    }

    // Update doc highlight
    _highlightDocRow(c);

    // Update entry panel
    const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
    const entryEl   = root.querySelector('#dim-entry-panel');
    if (!entryEl) return;

    const existingVal = _charState[c.id]?.actual ?? '';

    // Detail info rows (shared by LOT and numeric)
    const _stateNow  = _charState[c.id];
    const _savedNote  = _stateNow?.note  ?? c.note ?? '';
    const _savedZones = _stateNow?.zones ?? [];
    const detailRows = [
      c.tooling          ? `<div class="dim-detail-row"><span class="dim-detail-lbl">Tooling</span><span class="dim-detail-val">${c.tooling}</span></div>` : '',
      c.bp_zone          ? `<div class="dim-detail-row"><span class="dim-detail-lbl">B/P Zone</span><span class="dim-detail-val">${c.bp_zone}</span></div>` : '',
      c.inspection_level ? `<div class="dim-detail-row"><span class="dim-detail-lbl">Insp. Level</span><span class="dim-detail-val">${c.inspection_level}</span></div>` : '',
      c.remarks          ? `<div class="dim-detail-row"><span class="dim-detail-lbl">Remarks</span><span class="dim-detail-val">${c.remarks}</span></div>` : '',
      _savedZones.length > 0 ? `<div class="dim-detail-row"><span class="dim-detail-lbl">Zones</span><span class="dim-detail-val">${_savedZones.map(z => `<span style="display:inline-block;padding:1px 5px;border-radius:3px;margin:1px;font-size:11px;background:${z.is_confirmed===false?'rgba(220,53,69,.15)':'rgba(56,161,105,.15)'};color:${z.is_confirmed===false?'#c0392b':'#276749'}">${z.zone_name}</span>`).join('')}</span></div>` : '',
      _savedNote ? `<div class="dim-detail-row"><span class="dim-detail-lbl">Not</span><span class="dim-detail-val" style="font-style:italic;color:var(--text-secondary)">${_savedNote}</span></div>` : '',
    ].join('');

    if (_isLot(c)) {
      entryEl.innerHTML = `
        <div class="dim-entry-header">
          <span class="dim-item-no">Item ${String(c.item_no).replace(/\s+/g, '')}</span>
        </div>
        <div class="dim-dimension">${c.dimension || '—'}</div>
        <div class="dim-limits" style="color:var(--text-secondary);font-size:12px;">
          Attribute check — no numeric measurement required.<br>
          Inspect visually and confirm conformance.
        </div>
        ${detailRows}
        ${pageCanWrite ? `<div class="dim-lot-btns">
          <button class="btn btn-primary dim-lot-btn" data-result="Conform">✓ Conform</button>
          <button class="btn btn-danger  dim-lot-btn" data-result="Not Conform">✗ Not Conform</button>
        </div>` : ''}
        <div class="dim-nav-row">
          <button class="btn btn-ghost btn-sm" id="dim-prev-btn">◀ Prev</button>
          <span class="dim-nav-counter">${index + 1} / ${_chars.length}</span>
          <button class="btn btn-ghost btn-sm" id="dim-next-btn">Next ▶</button>
          <button class="btn btn-secondary btn-sm" id="dim-open-detail-btn">📋 Detail</button>
        </div>
        <div id="dim-nc-panel"></div>
        <div class="dim-progress-wrap">
          <div class="dim-progress-bar-bg"><div class="dim-progress-bar" id="dim-progress-bar"></div></div>
          <span class="dim-progress-lbl" id="dim-progress-label"></span>
        </div>`;

      entryEl.querySelector('#dim-open-detail-btn')?.addEventListener('click', () =>
        _openDetailModal(_chars[_activeIndex]));

      entryEl.querySelectorAll('.dim-lot-btn').forEach(btn => {
        btn.addEventListener('click', () => _saveLot(c, btn.dataset.result));
      });

      // Render NC panel if already Not Conform
      if (_charState[c.id]?.result === 'fail') _renderNcActionPanel(c);
    } else {
      entryEl.innerHTML = `
        <div class="dim-entry-header">
          <span class="dim-item-no">Item ${String(c.item_no).replace(/\s+/g, '')}</span>
          ${c.badge ? `<span class="dim-badge">${c.badge}</span>` : ''}
        </div>
        <div class="dim-dimension">${c.dimension || '—'}</div>
        <div class="dim-limits">
          ${hasLimits
            ? `Limits: <span class="dim-limit-val">${formatLimits(c.lower_limit, c.upper_limit)}</span>`
            : '<span class="text-secondary">No limits defined</span>'}
        </div>
        ${detailRows}
        ${pageCanWrite ? `<div class="dim-input-row">
          <input type="text" class="form-input dim-actual-input" id="dim-actual-input"
                 placeholder="Enter measurement…" value="${existingVal}" autocomplete="off" />
          <span class="dim-tol-preview" id="dim-tol-preview"></span>
        </div>` : ''}
        <div class="dim-nav-row">
          <button class="btn btn-ghost btn-sm" id="dim-prev-btn">◀ Prev</button>
          <span class="dim-nav-counter">${index + 1} / ${_chars.length}</span>
          <button class="btn btn-ghost btn-sm" id="dim-next-btn">Next ▶</button>
          <button class="btn btn-secondary btn-sm" id="dim-open-detail-btn">📋 Detail</button>
        </div>
        <div id="dim-nc-panel"></div>
        <div class="dim-progress-wrap">
          <div class="dim-progress-bar-bg"><div class="dim-progress-bar" id="dim-progress-bar"></div></div>
          <span class="dim-progress-lbl" id="dim-progress-label"></span>
        </div>`;

      entryEl.querySelector('#dim-open-detail-btn')?.addEventListener('click', () =>
        _openDetailModal(_chars[_activeIndex]));

      // Render NC panel if already NOK
      if (_charState[c.id]?.result === 'fail') _renderNcActionPanel(c);

      const input   = entryEl.querySelector('#dim-actual-input');
      const preview = entryEl.querySelector('#dim-tol-preview');

      if (input) {
        function updatePreview(val) {
          if (!hasLimits || !val) { preview.textContent = ''; preview.className = 'dim-tol-preview'; return; }
          const allVals = val.split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v));
          if (allVals.length === 0) { preview.textContent = ''; return; }
          const ok = allVals.every(v => v >= c.lower_limit && v <= c.upper_limit);
          if (ok) {
            preview.textContent = '✓ Within Tolerance';
            preview.className   = 'dim-tol-preview dim-tol-ok';
          } else {
            preview.textContent = '✗ Out of Tolerance';
            preview.className   = 'dim-tol-preview dim-tol-fail';
          }
        }

        updatePreview(String(existingVal));

        input.addEventListener('input', () => updatePreview(input.value));
        input.addEventListener('keydown', async (e) => {
          if (e.key === 'Enter') { e.preventDefault(); await _saveNumeric(c, input.value); }
        });
        input.focus();
      }
    }

    // Nav buttons (always rendered)
    const prevBtn = entryEl.querySelector('#dim-prev-btn');
    const nextBtn = entryEl.querySelector('#dim-next-btn');
    if (prevBtn) { prevBtn.disabled = index === 0; prevBtn.addEventListener('click', () => _activateChar(index - 1)); }
    if (nextBtn) { nextBtn.disabled = index === _chars.length - 1; nextBtn.addEventListener('click', () => _activateChar(index + 1)); }

    _renderProgress();
  }

  // Returns { reason: 'typo'|'re-inspect' } or null if cancelled
  function _showUpdateConfirmDialog(oldVal, newVal) {
    return new Promise(resolve => {
      const ov = document.createElement('div');
      ov.className = 'modal-overlay';
      ov.style.cssText = 'position:fixed;inset:0;background:rgba(0,0,0,.5);z-index:9999;display:flex;align-items:center;justify-content:center';
      ov.innerHTML = `
        <div style="background:var(--bg-card,#fff);border-radius:8px;padding:24px;max-width:360px;width:90%;box-shadow:0 8px 32px rgba(0,0,0,.3)">
          <div style="font-weight:600;margin-bottom:12px">Değer Değiştiriliyor</div>
          <p style="margin-bottom:16px;color:var(--text-secondary,#666)">
            <strong>${oldVal}</strong> → <strong>${newVal}</strong>
          </p>
          <label style="display:block;margin-bottom:8px;font-size:13px">Değişiklik nedeni:</label>
          <select id="_upd-reason" style="width:100%;padding:8px;border:1px solid var(--border,#ddd);border-radius:4px;margin-bottom:12px">
            <option value="typo">Typo (yanlış giriş)</option>
            <option value="re-inspect">Re-inspect (yeniden ölçüm)</option>
          </select>
          <label style="display:block;margin-bottom:6px;font-size:13px">Not <span style="color:var(--text-secondary,#999);font-weight:400">(opsiyonel)</span>:</label>
          <textarea id="_upd-note" rows="3" placeholder="Açıklama ekleyin…"
            style="width:100%;padding:8px;border:1px solid var(--border,#ddd);border-radius:4px;resize:vertical;font-size:13px;margin-bottom:16px;box-sizing:border-box"></textarea>
          <div style="display:flex;gap:8px;justify-content:flex-end">
            <button id="_upd-cancel" class="btn btn-secondary">İptal</button>
            <button id="_upd-confirm" class="btn btn-primary">Onayla</button>
          </div>
        </div>`;
      document.body.appendChild(ov);
      ov.querySelector('#_upd-confirm').addEventListener('click', () => {
        const reason = ov.querySelector('#_upd-reason').value;
        const note   = ov.querySelector('#_upd-note').value.trim() || null;
        document.body.removeChild(ov);
        resolve({ reason, note });
      });
      ov.querySelector('#_upd-cancel').addEventListener('click', () => {
        document.body.removeChild(ov);
        resolve(null);
      });
    });
  }

  async function _saveNumeric(c, rawVal) {
    if (!rawVal) return;
    const allVals = rawVal.split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v));
    if (allVals.length === 0) { window.toast('Invalid measurement', 'error'); return; }

    const min = Math.min(...allVals), max = Math.max(...allVals);
    const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
    const isOk = !hasLimits || allVals.every(v => v >= c.lower_limit && v <= c.upper_limit);
    const resultStr = allVals.length > 1 ? `${min} / ${max}` : String(allVals[0]);

    // Detect update: if a value already exists and is different, ask for reason
    const existing = _charState[c.id]?.actual;
    let updateReason = null;
    let updateNote   = null;
    if (existing && existing !== resultStr) {
      const answer = await _showUpdateConfirmDialog(existing, resultStr);
      if (!answer) return;
      updateReason = answer.reason;
      updateNote   = answer.note;
    }

    try {
      await api.numericResults.deleteByCharacter(c.id);
      await api.numericResults.create({ character_id: c.id, actual: rawVal, part_label: null, update_reason: updateReason, update_note: updateNote });
      await api.characters.update(c.id, { inspection_result: resultStr });
      _charState[c.id] = { result: isOk ? 'ok' : 'fail', actual: resultStr };
      const numCharObj = _chars.find(ch => ch.id === c.id);
      if (numCharObj) numCharObj.inspection_result = resultStr;
      _updateCharListItem(_activeIndex);
      _renderStats();
      _renderProgress();
      _updateDocActualCell(c, resultStr, isOk);
      if (!isOk) _renderNcActionPanel(c);
      window.toast('Saved', 'success');
      if (isOk && _activeIndex < _chars.length - 1) _activateChar(_activeIndex + 1);
    } catch (err) {
      window.toast('Save error: ' + err.message, 'error');
    }
  }

  async function _saveLot(c, resultStr) {
    const isOk = resultStr === 'Conform';

    // Update detection: if existing result differs, ask for reason
    const existing = _charState[c.id]?.actual;
    let updateReason = null;
    let updateNote   = null;
    if (existing && existing !== resultStr) {
      const answer = await _showUpdateConfirmDialog(existing, resultStr);
      if (!answer) return;
      updateReason = answer.reason;
      updateNote   = answer.note;
    }

    try {
      await api.categoricalResults.deleteByCharacter(c.id);
      await api.categoricalResults.create({
        character_id: c.id,
        is_confirmed: isOk,
        update_reason: updateReason,
        update_note:   updateNote,
      });
      await api.characters.update(c.id, { inspection_result: resultStr });
      _charState[c.id] = { ...(_charState[c.id] || {}), result: isOk ? 'ok' : 'fail', actual: resultStr };
      const lotCharObj = _chars.find(ch => ch.id === c.id);
      if (lotCharObj) lotCharObj.inspection_result = resultStr;
      _updateCharListItem(_activeIndex);
      _renderStats();
      _renderProgress();
      _updateDocActualCell(c, resultStr, isOk);
      if (!isOk) _renderNcActionPanel(c);
      window.toast('Saved', 'success');
      if (isOk && _activeIndex < _chars.length - 1) _activateChar(_activeIndex + 1);
    } catch (err) {
      window.toast('Save error: ' + err.message, 'error');
    }
  }

  function _renderNcActionPanel(c) {
    const panel = document.getElementById('dim-nc-panel');
    if (!panel) return;
    const disp = _charDispState[c.id];
    if (disp) {
      const types = _dispTypeCache || [];
      const dt = types.find(t => t.code === disp.decision);
      panel.innerHTML = `
        <div style="background:var(--color-warning-bg,#fffbeb);border:1px solid var(--color-warning,#f59e0b);border-radius:6px;padding:10px 14px;margin-top:10px;font-size:13px">
          <span style="font-weight:600">Karar:</span>
          <span class="disp-badge disp-${dt?.css_class||'unknown'}" style="margin-left:6px">${dt?.label||disp.decision}</span>
          <span style="color:var(--text-secondary,#666);margin-left:8px;font-size:12px">${disp.entered_by||''}</span>
        </div>`;
    } else {
      const currentUser = session.get() || 'Inspector';
      panel.innerHTML = `
        <div class="nc-action-panel" style="background:var(--color-danger-bg,#fef2f2);border:1px solid var(--color-danger,#ef4444);border-radius:6px;padding:10px 14px;margin-top:10px">
          <div style="font-weight:600;color:var(--color-danger,#ef4444);font-size:13px;margin-bottom:8px">⚠ Tolerans dışı — Karar gerekli</div>
          <div style="display:flex;gap:6px;flex-wrap:wrap">
            <button class="btn btn-sm nc-disp-btn" data-disp="MRB_SUBMITTED" style="background:#7c3aed;color:#fff;border:none">MRB_ACCEPT</button>
            <button class="btn btn-sm nc-disp-btn" data-disp="KABUL_RESIM" style="background:#059669;color:#fff;border:none">ACCEPT_PER_BP</button>
            <button class="btn btn-sm nc-disp-btn" data-disp="SCRAP" style="background:#dc2626;color:#fff;border:none">SCRAP</button>
          </div>
        </div>`;
      panel.querySelectorAll('.nc-disp-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
          const dispCode = btn.dataset.disp;
          btn.disabled = true;
          try {
            await api.characters.addDisposition(c.id, { decision: dispCode, entered_by: currentUser, note: '' });
            _charDispState[c.id] = { decision: dispCode, entered_by: currentUser };
            const idx = _chars.findIndex(ch => ch.id === c.id);
            if (idx >= 0) _updateCharListItem(idx);
            _renderNcActionPanel(c);
            window.toast('Karar kaydedildi.', 'success');
          } catch (err) {
            window.toast('Hata: ' + err.message, 'error');
            btn.disabled = false;
          }
        });
      });
    }
  }

  async function _openCharacterDispositionModal(c, onDone) {
    const today = new Date().toISOString().split('T')[0];

    // Fetch existing dispositions
    let existingDisps = [];
    try { existingDisps = await api.characters.listDispositions(c.id); } catch { /* none */ }
    const active = existingDisps
      .filter(d => d.decision !== 'VOID')
      .sort((a, b) => new Date(b.created_at) - new Date(a.created_at))[0] || null;
    const currentDecision = active?.decision || null;

    const allowed = await allowedNextDecisions(currentDecision);
    const types   = await _loadDispTypes();
    const decisionOptions = [
      { value: '', label: '— Karar Seçiniz —' },
      ...types
        .filter(t => t.active && (allowed.length === 0 || allowed.includes(t.code)))
        .sort((a, b) => (a.sort_order ?? 99) - (b.sort_order ?? 99))
        .map(t => ({ value: t.code, label: t.label })),
    ];

    // Measurement context
    let numericResults = [];
    try { numericResults = await api.numericResults.list(c.id); } catch { /* none */ }
    const hasLimits = (c.lower_limit !== 0 || c.upper_limit !== 0);
    let measurementHtml = `<div style="font-size:12px;color:var(--text-secondary);margin-bottom:12px">📐 ${c.dimension || 'Dimension'}</div>`;
    if (numericResults.length > 0) {
      const byPart = {};
      for (const r of numericResults) {
        if (!byPart[r.part_label] || new Date(r.created_at) > new Date(byPart[r.part_label].created_at))
          byPart[r.part_label] = r;
      }
      const rows = Object.values(byPart).map(r => {
        const nums = String(r.actual).split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v));
        const isOk = !hasLimits || (nums.length > 0 && nums.every(v => v >= c.lower_limit && v <= c.upper_limit));
        return `<tr><td style="padding:2px 6px 2px 0">${r.part_label || '—'}</td>
          <td style="font-weight:600;color:${isOk ? 'var(--success)' : 'var(--danger)'}">
            ${r.actual} ${isOk ? '✓' : '✗'}
          </td></tr>`;
      }).join('');
      measurementHtml = `
        <div class="disp-history-box" style="margin-bottom:12px">
          <strong style="font-size:12px;display:block;margin-bottom:4px">📐 ${c.dimension || 'Dimension'}</strong>
          ${hasLimits ? `<span style="font-size:11px;color:var(--text-secondary)">Limits: ${c.lower_limit} — ${c.upper_limit}</span>` : ''}
          <table style="width:100%;margin-top:6px;font-size:12px;border-collapse:collapse">
            <tr style="color:var(--text-secondary)"><th style="text-align:left;padding:2px 6px 2px 0">Part</th><th style="text-align:left">Actual</th></tr>
            ${rows}
          </table>
        </div>`;
    }

    window.modal.open(`
      <div class="modal-card">
        <div class="modal-header">
          <span class="modal-title">⚖ Disposition — Item ${String(c.item_no).replace(/\s+/g, '')}</span>
          <button class="modal-close" id="dp-close">&times;</button>
        </div>

        ${active ? `
          <div class="disp-history-box" style="display:flex;align-items:center;gap:8px">
            ${dispositionBadge(active)}
            ${active.note ? `<span class="disp-note-text">${active.note}</span>` : ''}
            ${pageCanWrite ? `<button class="btn btn-ghost btn-xs" id="dp-void-btn" style="margin-left:auto">✕ Düzelt</button>` : ''}
          </div>` : ''}

        ${measurementHtml}

        <form id="dp-form" novalidate>
          <div class="form-group" style="margin-bottom:14px;">
            <label>Karar *</label>
            <select class="form-select" name="decision" id="dp-decision" required>
              ${decisionOptions.map(o => `<option value="${o.value}">${o.label}</option>`).join('')}
            </select>
          </div>

          <div class="form-grid" style="margin-bottom:14px;">
            <div class="form-group">
              <label>Kararı Veren Mühendis</label>
              <input type="text" class="form-input" name="engineer" placeholder="Mühendis adı soyadı" />
            </div>
            <div class="form-group">
              <label>Sicil No (Sisteme Giren) *</label>
              <input type="text" class="form-input" name="entered_by"
                     placeholder="Inspector sicil no" value="${session.get() || ''}" />
            </div>
          </div>
          <div class="form-group" style="margin-bottom:14px;">
            <label>Karar Tarihi *</label>
            <input type="date" class="form-input" name="decided_at" value="${today}" required />
          </div>

          <div id="dp-fields-USE_AS_IS" class="dp-extra-fields" style="display:none;">
            <div class="form-group" style="margin-bottom:14px;">
              <label>Spec / Doküman No</label>
              <input type="text" class="form-input" name="spec_ref" placeholder="ör. ENG-SPEC-4421" />
            </div>
          </div>
          <div id="dp-fields-KABUL_RESIM" class="dp-extra-fields" style="display:none;">
            <div class="form-group" style="margin-bottom:14px;">
              <label>Resim / Çizim No</label>
              <input type="text" class="form-input" name="spec_ref" placeholder="ör. DRW-2026-0042" />
            </div>
          </div>
          <div id="dp-fields-CONFORMS" class="dp-extra-fields" style="display:none;">
            <p class="text-secondary" style="font-size:12px;padding:4px 0;">Inspector onayı yeterlidir, mühendis kararı gerekmez.</p>
          </div>
          <div id="dp-fields-REWORK" class="dp-extra-fields" style="display:none;">
            <p class="text-secondary" style="font-size:12px;padding:4px 0;">Parça yeniden işleme gönderildi. İşlem sonrası yeniden ölçüm yapılacak.</p>
          </div>
          <div id="dp-fields-VOID" class="dp-extra-fields" style="display:none;">
            <div class="form-group" style="margin-bottom:14px;">
              <label>Void Nedeni</label>
              <input type="text" class="form-input" name="void_reason" placeholder="Neden void?" />
            </div>
          </div>
          <div id="dp-fields-SCRAP" class="dp-extra-fields" style="display:none;">
            <div class="form-group" style="margin-bottom:14px;">
              <label>Hurda Nedeni</label>
              <input type="text" class="form-input" name="scrap_reason" placeholder="Hurda gerekçesi" />
            </div>
          </div>

          <div class="form-group" style="margin-bottom:14px;">
            <label>Not</label>
            <input type="text" class="form-input" name="note" placeholder="Opsiyonel not…" />
          </div>

          <div style="display:flex;gap:8px;justify-content:flex-end;margin-top:16px;">
            <button type="button" class="btn btn-secondary" id="dp-cancel">İptal</button>
            <button type="submit" class="btn btn-primary" id="dp-submit">Kaydet</button>
          </div>
        </form>
      </div>
    `);

    document.getElementById('dp-close').addEventListener('click', () => window.modal.close());
    document.getElementById('dp-cancel').addEventListener('click', () => window.modal.close());

    // Düzelt / Void
    document.getElementById('dp-void-btn')?.addEventListener('click', async () => {
      if (!confirm('Bu kararı geçersiz sayıp yeniden girmek istiyor musunuz?')) return;
      try {
        await api.characters.addDisposition(c.id, {
          decision: 'VOID', entered_by: session.get() || 'Inspector',
          decided_at: today, note: 'Düzeltme — önceki karar iptal'
        });
        _charDispState[c.id] = null;
        window.modal.close();
        _openCharacterDispositionModal(c, onDone);
      } catch (err) { window.toast('Hata: ' + err.message, 'error'); }
    });

    // Show/hide extra fields
    const decisionSel = document.getElementById('dp-decision');
    function showFields(val) {
      document.querySelectorAll('.dp-extra-fields').forEach(el => el.style.display = 'none');
      if (val) { const el = document.getElementById(`dp-fields-${val}`); if (el) el.style.display = ''; }
    }
    decisionSel.addEventListener('change', () => showFields(decisionSel.value));

    document.getElementById('dp-form').addEventListener('submit', async (e) => {
      e.preventDefault();
      const btn = document.getElementById('dp-submit');
      const fd  = new FormData(e.target);
      const decision   = fd.get('decision');
      const entered_by = (fd.get('entered_by') || '').trim();
      const decided_at = fd.get('decided_at');

      if (!decision)   { window.toast('Karar seçiniz.', 'error'); return; }
      if (!entered_by) { window.toast('Sicil no zorunludur.', 'error'); return; }

      const payload = { decision, entered_by, decided_at };
      const engineer    = (fd.get('engineer')    || '').trim(); if (engineer)    payload.engineer    = engineer;
      const note        = (fd.get('note')        || '').trim(); if (note)        payload.note        = note;
      const spec_ref    = (fd.get('spec_ref')    || '').trim(); if (spec_ref)    payload.spec_ref    = spec_ref;
      const void_reason = (fd.get('void_reason') || '').trim(); if (void_reason) payload.void_reason = void_reason;
      const scrap_reason= (fd.get('scrap_reason')|| '').trim(); if (scrap_reason)payload.scrap_reason= scrap_reason;

      // REWORK kararı verilirken mevcut ölçümleri snapshot olarak kaydet
      if (decision === 'REWORK' && numericResults.length > 0) {
        payload.measurements_snapshot = JSON.stringify(
          numericResults.map(r => ({ part_label: r.part_label, actual: r.actual }))
        );
      }

      btn.disabled = true;
      btn.textContent = 'Kaydediliyor...';
      try {
        await api.characters.addDisposition(c.id, payload);
        _charDispState[c.id] = { decision, entered_by, decided_at, note: payload.note || '' };
        const idx = _chars.findIndex(ch => ch.id === c.id);
        if (idx >= 0) _updateCharListItem(idx);
        window.toast('Disposition kaydedildi.', 'success');
        window.modal.close();
        if (onDone) await onDone();
      } catch (err) {
        window.toast('Hata: ' + err.message, 'error');
        btn.disabled = false;
        btn.textContent = 'Kaydet';
      }
    });
  }

  function _attachZoomPan(el) {
    let scale = 1, tx = 0, ty = 0;
    let dragging = false, lastX = 0, lastY = 0;
    let lastDist = 0;

    el.style.transformOrigin = '0 0';
    el.style.display = 'inline-block';
    el.style.minWidth = '100%';

    function apply() {
      el.style.transform = `translate(${tx}px,${ty}px) scale(${scale})`;
    }

    // Mouse wheel zoom (only when Ctrl is held)
    el.addEventListener('wheel', e => {
      if (!e.ctrlKey) return;
      e.preventDefault();
      const factor = e.deltaY < 0 ? 1.1 : 0.9;
      scale = Math.min(4, Math.max(0.3, scale * factor));
      apply();
    }, { passive: false });

    // Mouse drag pan
    el.addEventListener('mousedown', e => { dragging = true; lastX = e.clientX; lastY = e.clientY; el.style.cursor = 'grabbing'; });
    window.addEventListener('mousemove', e => {
      if (!dragging) return;
      tx += e.clientX - lastX; ty += e.clientY - lastY;
      lastX = e.clientX; lastY = e.clientY; apply();
    });
    window.addEventListener('mouseup', () => { dragging = false; el.style.cursor = 'grab'; });

    // Touch pinch + drag
    el.addEventListener('touchmove', e => {
      if (e.touches.length === 2) {
        e.preventDefault();
        const d = Math.hypot(e.touches[0].clientX - e.touches[1].clientX,
                             e.touches[0].clientY - e.touches[1].clientY);
        if (lastDist) scale = Math.min(4, Math.max(0.3, scale * d / lastDist));
        lastDist = d; apply();
      } else if (e.touches.length === 1 && dragging) {
        tx += e.touches[0].clientX - lastX; ty += e.touches[0].clientY - lastY;
        lastX = e.touches[0].clientX; lastY = e.touches[0].clientY; apply();
      }
    }, { passive: false });
    el.addEventListener('touchstart', e => {
      if (e.touches.length === 1) { dragging = true; lastX = e.touches[0].clientX; lastY = e.touches[0].clientY; }
      lastDist = 0;
    });
    el.addEventListener('touchend', () => { dragging = false; lastDist = 0; });

    // Toolbar zoom buttons (wired after insertion)
    const zoomIn    = () => { scale = Math.min(4, scale * 1.2); apply(); };
    const zoomOut   = () => { scale = Math.max(0.3, scale / 1.2); apply(); };
    const zoomReset = () => { scale = 1; tx = 0; ty = 0; apply(); };
    document.getElementById('dim-doc-zoom-in-btn')?.addEventListener('click', zoomIn);
    document.getElementById('dim-doc-zoom-out-btn')?.addEventListener('click', zoomOut);
    document.getElementById('dim-doc-zoom-reset-btn')?.addEventListener('click', zoomReset);
  }

  async function _loadDocViewer() {
    const viewerEl = root.querySelector('#dim-doc-viewer');
    if (!viewerEl) return;

    // Try to fetch the op sheet docx
    const docUrl = `/op-sheets/${id}.docx`;
    try {
      const resp = await fetch(docUrl);
      if (!resp.ok) throw new Error('not found');
      const arrayBuffer = await resp.arrayBuffer();
      if (typeof mammoth === 'undefined') throw new Error('mammoth not loaded');
      const result = await mammoth.convertToHtml({ arrayBuffer });
      _docHtml = result.value;
      viewerEl.innerHTML = `
        <div class="dim-doc-toolbar" style="display:flex;gap:6px;padding:4px 8px;border-bottom:1px solid var(--border);background:var(--bg-secondary);position:sticky;top:0;z-index:10;">
          <button id="dim-doc-zoom-in-btn" class="btn btn-ghost btn-xs" title="Zoom In">+</button>
          <button id="dim-doc-zoom-out-btn" class="btn btn-ghost btn-xs" title="Zoom Out">−</button>
          <button id="dim-doc-zoom-reset-btn" class="btn btn-ghost btn-xs" title="Reset Zoom">⌂</button>
          <button id="dim-doc-newtab-btn" class="btn btn-ghost btn-xs" style="margin-left:auto;" title="Open in new tab">↗ Tam Ekran</button>
        </div>
        <div class="dim-doc-inner" style="cursor:grab;overflow:visible;">${_docHtml}</div>`;

      document.getElementById('dim-doc-newtab-btn')?.addEventListener('click', () => {
        try {
          const blob = new Blob([`<html><body style="margin:0;background:#fff">${_docHtml}</body></html>`],
                                { type: 'text/html' });
          const url = URL.createObjectURL(blob);
          const w = window.open(url, '_blank');
          if (!w) window.toast('Popup engellendi. Tarayıcı izinlerini kontrol edin.', 'warning');
        } catch (err) {
          window.toast('Açılamadı: ' + err.message, 'error');
        }
      });

      _attachZoomPan(viewerEl.querySelector('.dim-doc-inner'));

      // Build row map: normalized item_no → <tr> (scan all cells for item-no pattern)
      _docRowMap = {};
      viewerEl.querySelectorAll('table tr').forEach(tr => {
        const cells = tr.querySelectorAll('td, th');
        cells.forEach(td => {
          const txt = td.textContent.replace(/\s+/g, '').toUpperCase();
          if (txt && !_docRowMap[txt]) _docRowMap[txt] = tr;
        });
      });
    } catch {
      viewerEl.innerHTML = `
        <div class="dim-doc-empty">
          <p>No op sheet uploaded for this inspection.</p>
          ${pageCanWrite ? `
          <label class="btn btn-ghost btn-sm" for="dim-op-sheet-input" style="margin-top:8px;">
            📁 Upload Op Sheet (.docx)
          </label>
          <input type="file" id="dim-op-sheet-input" class="file-input-hidden" accept=".docx" />` : ''}
        </div>`;
      // wire upload if rendered here
      root.querySelector('#dim-op-sheet-input')?.addEventListener('change', async (e) => {
        const file = e.target.files[0];
        if (!file) return;
        e.target.value = '';
        try {
          const result = await api.inspections.parseOpSheet(id, file);
          window.toast(`${result.characters_created} characteristics created.`, 'success');
          _dimLoaded = false;
          await loadDimensional();
        } catch (err) {
          window.toast('Parse error: ' + err.message, 'error');
        }
      });
    }
  }

  // ── Detail Modal ─────────────────────────────────────────────────────────────

  function _getOrCreateModalOverlay() {
    let ov = document.getElementById('dim-modal-overlay');
    if (!ov) {
      ov = document.createElement('div');
      ov.id = 'dim-modal-overlay';
      ov.className = 'dim-modal-overlay hidden';
      document.body.appendChild(ov);
    }
    return ov;
  }

  function _closeDetailModal() {
    const ov = document.getElementById('dim-modal-overlay');
    if (ov) ov.className = 'dim-modal-overlay hidden';
    if (_activeDimModalKeyHandler) {
      document.removeEventListener('keydown', _activeDimModalKeyHandler);
      _activeDimModalKeyHandler = null;
    }
  }

  function _buildPartRowsDimensional(c, count, existingResults, labelOverrides) {
    const baseSerial = detail.serial_number || 'Part';
    let html = '';
    for (let i = 1; i <= count; i++) {
      const existing = existingResults[i - 1];
      const val = existing?.actual ?? '';
      const label = (labelOverrides && labelOverrides[i - 1]) || existing?.part_label || `${baseSerial}-${i}`;
      const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
      let statusHtml = '';
      if (val) {
        const allVals = val.split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v));
        if (allVals.length > 0 && hasLimits) {
          const ok = allVals.every(v => v >= c.lower_limit && v <= c.upper_limit);
          statusHtml = ok
            ? `<span class="dm-part-status ok">✓ OK</span>`
            : `<span class="dm-part-status fail">✗ OOT</span>`;
        }
      }
      html += `
        <div class="dm-part-row" data-part-index="${i}">
          <span class="dm-part-label" data-part-idx="${i}" title="Double-click to edit label">${label}</span>
          <input type="text" class="form-input dm-part-input" data-part="${i}"
                 placeholder="e.g. 15.5 or 15/16" value="${val}" autocomplete="off" />
          <span class="dm-part-info">${statusHtml}</span>
        </div>`;
    }
    return html;
  }

  function _buildPartRowsLot(count, existingResults) {
    const baseSerial = detail.serial_number || 'Part';
    let html = '';
    for (let i = 1; i <= count; i++) {
      const existing = existingResults[i - 1];
      const isConform = existing ? existing.is_confirmed : true;
      const info = existing?.additional_info ?? '';
      const label = `${baseSerial}-${i}`;
      html += `
        <div class="dm-part-row" data-part-index="${i}">
          <span class="dm-part-label">${label}</span>
          <select class="dm-conform-select" data-part="${i}">
            <option value="1" ${isConform ? 'selected' : ''}>Conform</option>
            <option value="0" ${!isConform ? 'selected' : ''}>Not Conform</option>
          </select>
          <input type="text" class="form-input dm-part-info" data-part="${i}"
                 placeholder="Info…" value="${info}" />
        </div>`;
    }
    return html;
  }

  // Zone rows for LOT/Attribute characters (categorical conform/not conform)
  function _buildZoneRows(existingZones) {
    if (!existingZones.length) return '';
    return existingZones.map((z, i) => `
      <div class="dm-zone-row" data-zone-index="${i}" style="display:flex;gap:6px;align-items:center;margin-bottom:4px;">
        <input type="text" class="form-input dm-zone-name" data-zone="${i}"
               placeholder="Zone…" value="${z.zone_name || ''}" style="width:80px;" />
        <input type="text" class="form-input dm-zone-value" data-zone="${i}"
               placeholder="Info…" value="${z.additional_info || ''}" style="width:100px;" />
        <label style="display:flex;align-items:center;gap:4px;font-size:12px;white-space:nowrap;">
          <input type="checkbox" class="dm-conform-check" data-zone="${i}"
                 ${z.is_confirmed !== false ? 'checked' : ''}> OK
        </label>
      </div>`).join('');
  }

  // Zone rows for Dimensional characters (numeric measurement per zone)
  function _buildZoneRowsDimensional(c, existingZones) {
    if (!existingZones.length) return '';
    const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
    return existingZones.map((z, i) => {
      const val = z.actual ?? z.additional_info ?? '';
      let statusHtml = '';
      if (val && hasLimits) {
        const v = parseFloat(val);
        if (!isNaN(v)) {
          statusHtml = (v >= c.lower_limit && v <= c.upper_limit)
            ? `<span class="dm-part-status ok">✓ OK</span>`
            : `<span class="dm-part-status fail">✗ OOT</span>`;
        }
      }
      return `
      <div class="dm-zone-row" data-zone-index="${i}" style="display:flex;gap:6px;align-items:center;margin-bottom:4px;">
        <input type="text" class="form-input dm-zone-name" data-zone="${i}"
               placeholder="Zone adı…" value="${z.zone_name || z.part_label || ''}" style="width:90px;" />
        <input type="text" class="form-input dm-zone-actual" data-zone="${i}"
               placeholder="Ölçüm…" value="${val}" autocomplete="off" style="width:100px;" />
        <span class="dm-part-info">${statusHtml}</span>
      </div>`;
    }).join('');
  }

  function _collectZoneRowsDimensional() {
    return Array.from(document.querySelectorAll('#dm-zones-container .dm-zone-row'))
      .map(row => ({
        zone_name: row.querySelector('.dm-zone-name')?.value.trim() ?? '',
        actual:    row.querySelector('.dm-zone-actual')?.value.trim() ?? '',
      }))
      .filter(z => z.zone_name && z.actual);
  }

  async function _openDetailModal(c) {
    const ov = _getOrCreateModalOverlay();
    ov.innerHTML = '<div class="dim-modal-card"><div class="dim-modal-body" style="padding:20px;text-align:center;">Loading…</div></div>';
    ov.className = 'dim-modal-overlay';

    // Close on overlay click
    ov.addEventListener('click', (e) => { if (e.target === ov) _closeDetailModal(); }, { once: true });

    // Fetch existing results
    let existingNumeric = [], existingCategorical = [], existingZones = [], existingDispositions = [];
    try {
      if (!_isLot(c)) {
        [existingNumeric, existingDispositions] = await Promise.all([
          api.numericResults.list(c.id),
          api.characters.listDispositions(c.id),
        ]);
      } else {
        [existingCategorical, existingZones] = await Promise.all([
          api.categoricalResults.list(c.id),
          api.zoneResults.list(c.id),
        ]);
      }
    } catch { /* use empty arrays */ }

    // For dimensional: split NumericPartResults into serial-based parts vs named zones
    // Zone labels are user-defined names (not matching the auto "Serial-N" pattern)
    let existingParts = existingNumeric;
    let existingZoneNumerics = [];
    if (!_isLot(c) && existingNumeric.length > 0) {
      const baseSerial = detail.serial_number || 'Part';
      const serialRe   = new RegExp(`^${baseSerial.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}-\\d+$`);
      existingParts        = existingNumeric.filter(r => !r.part_label || serialRe.test(r.part_label));
      existingZoneNumerics = existingNumeric.filter(r =>  r.part_label && !serialRe.test(r.part_label));
    }

    // REWORK snapshot: latest non-void REWORK disposition with measurements_snapshot
    const reworkSnapshotDisp = [...existingDispositions]
      .filter(d => d.decision === 'REWORK' && d.decision !== 'VOID' && d.measurements_snapshot)
      .sort((a, b) => new Date(b.created_at) - new Date(a.created_at))[0] || null;

    let preReworkMeasurements = null;
    if (reworkSnapshotDisp) {
      try { preReworkMeasurements = JSON.parse(reworkSnapshotDisp.measurements_snapshot); } catch { /* ignore */ }
    }

    // Eğer REWORK snapshot varsa re-inspect için boş satırlarla başla
    const reinspectMode = !!preReworkMeasurements;
    const partCount = Math.max(1, _isLot(c)
      ? existingCategorical.length || 1
      : reinspectMode
        ? (preReworkMeasurements.length || 1)   // snapshot'taki part sayısını kullan, rows boş başlasın
        : existingParts.length || 1);
    const zoneCount = existingZones.length || 0;
    const noteVal = c.note || '';

    if (!_isLot(c)) {
      const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
      const limitsHtml = hasLimits
        ? `<div class="dim-modal-limits">Limits: ${formatLimits(c.lower_limit, c.upper_limit)}</div>`
        : `<div class="dim-modal-limits" style="color:var(--text-muted)">No limits defined</div>`;

      ov.innerHTML = `
        <div class="dim-modal-card">
          <div class="dim-modal-header">
            <span>Detail — Item ${c.item_no}</span>
            <button class="dim-modal-close" id="dm-close-btn">✕</button>
          </div>
          <div class="dim-modal-body">
            <div class="dim-modal-info">
              <div class="dim-modal-dimension">${c.dimension || '—'}</div>
              ${limitsHtml}
              <div class="dim-modal-meta-grid">
                ${c.tooling          ? `<span>Tooling</span><span>${c.tooling}</span>` : ''}
                ${c.bp_zone          ? `<span>B/P Zone</span><span>${c.bp_zone}</span>` : ''}
                ${c.inspection_level ? `<span>Insp. Level</span><span>${c.inspection_level}</span>` : ''}
                ${c.remarks          ? `<span>Remarks</span><span>${c.remarks}</span>` : ''}
              </div>
            </div>
            ${preReworkMeasurements ? `
            <div class="dim-modal-section" style="background:rgba(245,158,11,0.08);border:1px solid rgba(245,158,11,0.3);border-radius:6px;padding:8px 12px;margin-bottom:8px;">
              <span style="color:#f59e0b;font-weight:600;">🔄 Round 1 — Pre-Rework</span>
            </div>
            <div style="padding:4px 0 12px;opacity:0.7;font-size:12px;">
              ${preReworkMeasurements.map(r => {
                const label = r.part_label || '—';
                const vals = String(r.actual).split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v));
                const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
                const isOk = !hasLimits || (vals.length > 0 && vals.every(v => v >= c.lower_limit && v <= c.upper_limit));
                return `<div class="dm-part-row" style="pointer-events:none;">
                  <span class="dm-part-label">${label}</span>
                  <span class="form-input dm-part-input" style="display:inline-block;line-height:2;color:var(--text-secondary);">${r.actual}</span>
                  <span class="dm-part-status ${isOk ? 'ok' : 'fail'}">${isOk ? '✓ OK' : '✗ OOT'}</span>
                </div>`;
              }).join('')}
            </div>
            <div class="dim-modal-section" style="background:rgba(56,161,105,0.08);border:1px solid rgba(56,161,105,0.3);border-radius:6px;padding:8px 12px;margin-bottom:8px;">
              <span style="color:#68d391;font-weight:600;">🔁 Round 2 — Re-Inspect</span>
            </div>` : ''}
            <div class="dim-modal-section">
              <span>📦 Parts</span>
              <button class="dm-count-btn" id="dm-part-dec">−</button>
              <span class="dm-count-val" id="dm-part-count">${partCount}</span>
              <button class="dm-count-btn" id="dm-part-inc">+</button>
            </div>
            <div id="dm-parts-container">
              ${_buildPartRowsDimensional(c, partCount, reinspectMode ? [] : existingParts,
                reinspectMode ? preReworkMeasurements.map(r => r.part_label) : null)}
            </div>
            <div class="dim-modal-section">
              <span>📐 Zones (bölgesel ölçümler)</span>
              <button class="dm-count-btn" id="dm-zone-dec">−</button>
              <span class="dm-count-val" id="dm-zone-count">${existingZoneNumerics.length}</span>
              <button class="dm-count-btn" id="dm-zone-inc">+</button>
            </div>
            <div id="dm-zones-container">
              ${_buildZoneRowsDimensional(c, existingZoneNumerics)}
            </div>
            <div class="dim-modal-section">
              <label>📝 Notes</label>
            </div>
            <textarea id="dm-notes" rows="2">${noteVal}</textarea>
            <div class="dim-modal-section" style="margin-top:10px">
              <span>⚖ Disposition</span>
              ${pageCanWrite ? `<button class="btn btn-primary btn-sm" id="dm-disp-open-btn" style="margin-left:auto">⚖ Karar Ekle / Düzenle</button>` : ''}
            </div>
            <div id="dm-disp-list">
              ${existingDispositions.length > 0
                ? existingDispositions.map(d => {
                    const dt = (_dispTypeCache||[]).find(t => t.code === d.decision);
                    return `<div class="dim-disp-row" style="display:flex;align-items:center;gap:6px;margin-bottom:4px">
                      <span class="disp-badge disp-${dt?.css_class||'unknown'}">${dt?.label||d.decision}</span>
                      <span style="font-size:11px;color:var(--text-secondary)">${d.entered_by || ''}</span>
                      ${d.note ? `<span style="font-size:11px;color:var(--text-secondary)">— ${d.note}</span>` : ''}
                    </div>`;
                  }).join('')
                : '<p style="color:var(--text-secondary);font-size:12px;margin:4px 0 8px">Henüz karar yok.</p>'}
            </div>
          </div>
          <div class="dim-modal-footer">
            ${pageCanWrite ? `
            <button id="dm-manual-accept" class="btn btn-success">✓ Manual Accept</button>
            <button id="dm-manual-reject" class="btn btn-danger">✗ Manual Reject</button>
            <button id="dm-save" class="btn btn-primary">💾 Save</button>` : ''}
          </div>
        </div>`;

      let currentCount = partCount;
      let currentZoneCount = existingZoneNumerics.length;

      document.getElementById('dm-close-btn').addEventListener('click', _closeDetailModal);

      if (_activeDimModalKeyHandler) {
        document.removeEventListener('keydown', _activeDimModalKeyHandler);
        _activeDimModalKeyHandler = null;
      }
      function _dimModalKeyHandler(e) {
        if (e.key === 'Enter' && !e.shiftKey && e.target.tagName !== 'TEXTAREA' && e.target.tagName !== 'BUTTON') {
          e.preventDefault();
          document.getElementById('dm-save')?.click();
        }
      }
      _activeDimModalKeyHandler = _dimModalKeyHandler;
      document.addEventListener('keydown', _dimModalKeyHandler);

      document.getElementById('dm-part-dec').addEventListener('click', () => {
        if (currentCount <= 1) return;
        currentCount--;
        document.getElementById('dm-part-count').textContent = currentCount;
        document.getElementById('dm-parts-container').innerHTML =
          _buildPartRowsDimensional(c, currentCount, _collectDimPartRows());
      });

      document.getElementById('dm-part-inc').addEventListener('click', () => {
        currentCount++;
        document.getElementById('dm-part-count').textContent = currentCount;
        document.getElementById('dm-parts-container').innerHTML =
          _buildPartRowsDimensional(c, currentCount, _collectDimPartRows());
      });

      // Double-click part label to edit inline
      document.getElementById('dm-parts-container').addEventListener('dblclick', e => {
        const lbl = e.target.closest('.dm-part-label');
        if (!lbl) return;
        const current = lbl.textContent;
        const inp = document.createElement('input');
        inp.className = 'form-input dm-part-label-edit';
        inp.value = current;
        inp.style.width = '90px';
        lbl.replaceWith(inp);
        inp.focus(); inp.select();
        inp.addEventListener('blur', () => {
          const span = document.createElement('span');
          span.className = 'dm-part-label';
          span.textContent = inp.value.trim() || current;
          span.title = 'Double-click to edit label';
          inp.replaceWith(span);
        });
        inp.addEventListener('keydown', e2 => { if (e2.key === 'Enter') inp.blur(); });
      });

      document.getElementById('dm-zone-dec').addEventListener('click', () => {
        if (currentZoneCount <= 0) return;
        currentZoneCount--;
        document.getElementById('dm-zone-count').textContent = currentZoneCount;
        const collected = _collectZoneRowsDimensional();
        collected.splice(currentZoneCount);
        document.getElementById('dm-zones-container').innerHTML = _buildZoneRowsDimensional(c, collected);
      });
      document.getElementById('dm-zone-inc').addEventListener('click', () => {
        currentZoneCount++;
        document.getElementById('dm-zone-count').textContent = currentZoneCount;
        const collected = _collectZoneRowsDimensional();
        while (collected.length < currentZoneCount)
          collected.push({ zone_name: '', actual: '' });
        document.getElementById('dm-zones-container').innerHTML = _buildZoneRowsDimensional(c, collected);
      });

      document.getElementById('dm-manual-accept')?.addEventListener('click', () =>
        _saveModalDimensional(c, 'Manual Accept'));
      document.getElementById('dm-manual-reject')?.addEventListener('click', () =>
        _saveModalDimensional(c, 'Manual Reject'));
      document.getElementById('dm-save')?.addEventListener('click', () =>
        _saveModalDimensional(c, null));

      document.getElementById('dm-disp-open-btn')?.addEventListener('click', () => {
        _closeDetailModal();
        _openCharacterDispositionModal(c, async () => {
          await _openDetailModal(c);
        });
      });

    } else {
      // LOT modal
      ov.innerHTML = `
        <div class="dim-modal-card">
          <div class="dim-modal-header">
            <span>Detail — Item ${c.item_no}</span>
            <button class="dim-modal-close" id="dm-close-btn">✕</button>
          </div>
          <div class="dim-modal-body">
            <div class="dim-modal-info">
              <div class="dim-modal-dimension">${c.dimension || '—'}</div>
              <div class="dim-modal-limits" style="color:var(--text-muted);font-size:.9rem;">
                Categorical check — confirm conformance per part.
              </div>
              <div class="dim-modal-meta-grid">
                ${c.tooling          ? `<span>Tooling</span><span>${c.tooling}</span>` : ''}
                ${c.bp_zone          ? `<span>B/P Zone</span><span>${c.bp_zone}</span>` : ''}
                ${c.inspection_level ? `<span>Insp. Level</span><span>${c.inspection_level}</span>` : ''}
                ${c.remarks          ? `<span>Remarks</span><span>${c.remarks}</span>` : ''}
              </div>
            </div>
            <div class="dim-modal-section">
              <span>📦 Parts</span>
              <button class="dm-count-btn" id="dm-part-dec">−</button>
              <span class="dm-count-val" id="dm-part-count">${partCount}</span>
              <button class="dm-count-btn" id="dm-part-inc">+</button>
            </div>
            <div id="dm-parts-container">
              ${_buildPartRowsLot(partCount, existingCategorical)}
            </div>
            ${zoneCount > 0 ? `
            <div class="dim-modal-section">
              <span>🗺 Zones</span>
              <button class="dm-count-btn" id="dm-zone-dec">−</button>
              <span class="dm-count-val" id="dm-zone-count">${zoneCount}</span>
              <button class="dm-count-btn" id="dm-zone-inc">+</button>
            </div>
            <div id="dm-zones-container">
              ${_buildZoneRows(existingZones)}
            </div>` : `
            <div class="dim-modal-section">
              <span>🗺 Zones</span>
              <button class="dm-count-btn" id="dm-zone-dec">−</button>
              <span class="dm-count-val" id="dm-zone-count">${zoneCount}</span>
              <button class="dm-count-btn" id="dm-zone-inc">+</button>
            </div>
            <div id="dm-zones-container"></div>`}
            <div class="dim-modal-section">
              <label>📝 Notes</label>
            </div>
            <textarea id="dm-notes" rows="2">${noteVal}</textarea>
          </div>
          <div class="dim-modal-footer">
            ${pageCanWrite ? `<button id="dm-save" class="btn btn-primary">💾 Save</button>` : ''}
          </div>
        </div>`;

      let currentPartCount = partCount;
      let currentZoneCount = zoneCount;

      document.getElementById('dm-close-btn').addEventListener('click', _closeDetailModal);

      document.getElementById('dm-part-dec').addEventListener('click', () => {
        if (currentPartCount <= 1) return;
        currentPartCount--;
        document.getElementById('dm-part-count').textContent = currentPartCount;
        document.getElementById('dm-parts-container').innerHTML =
          _buildPartRowsLot(currentPartCount, _collectLotPartRows());
      });
      document.getElementById('dm-part-inc').addEventListener('click', () => {
        currentPartCount++;
        document.getElementById('dm-part-count').textContent = currentPartCount;
        document.getElementById('dm-parts-container').innerHTML =
          _buildPartRowsLot(currentPartCount, _collectLotPartRows());
      });
      document.getElementById('dm-zone-dec').addEventListener('click', () => {
        if (currentZoneCount <= 0) return;
        currentZoneCount--;
        document.getElementById('dm-zone-count').textContent = currentZoneCount;
        const collected = _collectZoneRows();
        collected.splice(currentZoneCount);
        document.getElementById('dm-zones-container').innerHTML = _buildZoneRows(collected);
      });
      document.getElementById('dm-zone-inc').addEventListener('click', () => {
        currentZoneCount++;
        document.getElementById('dm-zone-count').textContent = currentZoneCount;
        const collected = _collectZoneRows();
        while (collected.length < currentZoneCount)
          collected.push({ zone_name: '', is_confirmed: true, additional_info: '' });
        document.getElementById('dm-zones-container').innerHTML = _buildZoneRows(collected);
      });

      document.getElementById('dm-save')?.addEventListener('click', () => _saveModalLot(c));
    }
  }

  function _collectDimPartRows() {
    return Array.from(document.querySelectorAll('#dm-parts-container .dm-part-row')).map(row => ({
      actual: row.querySelector('.dm-part-input')?.value ?? '',
      part_label: (row.querySelector('.dm-part-label')?.textContent
                ?? row.querySelector('.dm-part-label-edit')?.value
                ?? '').trim(),
    }));
  }

  function _collectLotPartRows() {
    return Array.from(document.querySelectorAll('#dm-parts-container .dm-part-row')).map(row => ({
      is_confirmed: row.querySelector('.dm-conform-select')?.value === '1',
      additional_info: row.querySelector('.dm-part-info')?.value ?? '',
    }));
  }

  function _collectZoneRows() {
    return Array.from(document.querySelectorAll('#dm-zones-container .dm-zone-row')).map(row => ({
      zone_name:      row.querySelector('.dm-zone-name')?.value ?? '',
      is_confirmed:   row.querySelector('.dm-conform-check')?.checked ?? true,
      additional_info: row.querySelector('.dm-zone-value')?.value ?? '',
    }));
  }

  async function _saveModalDimensional(c, overrideResult) {
    const parts      = _collectDimPartRows();
    const validParts = parts.filter(p => p.actual.trim());
    const zones      = _collectZoneRowsDimensional();   // { zone_name, actual }
    const noteVal    = document.getElementById('dm-notes')?.value ?? '';

    if (!overrideResult && validParts.length === 0 && zones.length === 0) {
      window.toast('En az bir ölçüm giriniz', 'error'); return;
    }

    // Compute min/max from both parts and zones
    const allVals = [
      ...validParts.flatMap(p => p.actual.split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v))),
      ...zones.map(z => parseFloat(z.actual)).filter(v => !isNaN(v)),
    ];
    let resultStr, isOk;
    if (overrideResult) {
      resultStr = overrideResult;
      isOk = overrideResult === 'Manual Accept';
    } else {
      if (allVals.length === 0) { window.toast('Invalid measurement values', 'error'); return; }
      const min = Math.min(...allVals), max = Math.max(...allVals);
      resultStr = min === max ? String(min) : `${min} / ${max}`;
      const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
      isOk = !hasLimits || allVals.every(v => v >= c.lower_limit && v <= c.upper_limit);
    }

    // Update detection (only for normal save, not override)
    let updateReason = null;
    let updateNote   = null;
    if (!overrideResult) {
      const existing = _charState[c.id]?.actual;
      if (existing && existing !== resultStr) {
        const answer = await _showUpdateConfirmDialog(existing, resultStr);
        if (!answer) return;
        updateReason = answer.reason;
        updateNote   = answer.note;
      }
    }

    try {
      await api.numericResults.deleteByCharacter(c.id);
      await api.zoneResults.deleteByCharacter(c.id);  // clean up any legacy categorical zone data
      const baseSerial = detail.serial_number || 'Part';
      for (let i = 0; i < validParts.length; i++) {
        await api.numericResults.create({
          character_id: c.id,
          actual: validParts[i].actual,
          part_label: validParts[i].part_label || `${baseSerial}-${i + 1}`,
          update_reason: updateReason,
          update_note:   updateNote,
        });
      }
      // Save zone measurements as NumericPartResults (zone_name → part_label)
      for (const z of zones) {
        await api.numericResults.create({
          character_id: c.id,
          actual: z.actual,
          part_label: z.zone_name,
          update_reason: updateReason,
          update_note:   updateNote,
        });
      }
      const updatePayload = { inspection_result: resultStr };
      if (noteVal) updatePayload.note = noteVal;
      await api.characters.update(c.id, updatePayload);

      // Store zone info for panel display
      const hasLimitsState = c.lower_limit !== 0 || c.upper_limit !== 0;
      const zonePanelData = zones.map(z => {
        const v = parseFloat(z.actual);
        const ok = !hasLimitsState || (isNaN(v) ? true : v >= c.lower_limit && v <= c.upper_limit);
        return { zone_name: z.zone_name, is_confirmed: ok };
      });
      _charState[c.id] = { result: isOk ? 'ok' : 'fail', actual: resultStr, zones: zonePanelData, note: noteVal };
      // Update local chars array
      const charObj = _chars.find(ch => ch.id === c.id);
      if (charObj) { charObj.note = noteVal; charObj.inspection_result = resultStr; }

      _updateCharListItem(_activeIndex);
      _renderStats();
      _renderProgress();
      _updateDocActualCell(c, resultStr, isOk);
      _closeDetailModal();
      _activateChar(_activeIndex);
      window.toast('Saved', 'success');
    } catch (err) {
      window.toast('Save error: ' + err.message, 'error');
    }
  }

  async function _saveModalLot(c) {
    const parts = _collectLotPartRows();
    const zones = _collectZoneRows().filter(z => z.zone_name.trim());
    const noteVal = document.getElementById('dm-notes')?.value ?? '';

    const hasNonConform = parts.some(p => !p.is_confirmed) || zones.some(z => !z.is_confirmed);
    const resultStr = hasNonConform ? 'Not Conform' : 'Conform';

    try {
      await api.categoricalResults.deleteByCharacter(c.id);
      await api.zoneResults.deleteByCharacter(c.id);

      for (const p of parts) {
        await api.categoricalResults.create({
          character_id: c.id,
          is_confirmed: p.is_confirmed,
          additional_info: p.additional_info || null,
          index: null,
        });
      }
      for (const z of zones) {
        await api.zoneResults.create({
          character_id: c.id,
          zone_name: z.zone_name,
          is_confirmed: z.is_confirmed,
          additional_info: z.additional_info || null,
        });
      }
      const updatePayload = { inspection_result: resultStr };
      if (noteVal) updatePayload.note = noteVal;
      await api.characters.update(c.id, updatePayload);

      const isOk = !hasNonConform;
      _charState[c.id] = { result: isOk ? 'ok' : 'fail', actual: resultStr, zones, note: noteVal };
      const charObj = _chars.find(ch => ch.id === c.id);
      if (charObj) { charObj.note = noteVal; charObj.inspection_result = resultStr; }

      _updateCharListItem(_activeIndex);
      _renderStats();
      _renderProgress();
      _updateDocActualCell(c, resultStr, isOk);
      _closeDetailModal();
      _activateChar(_activeIndex);
      window.toast('Saved', 'success');
    } catch (err) {
      window.toast('Save error: ' + err.message, 'error');
    }
  }

  // ─────────────────────────────────────────────────────────────────────────────

  async function loadDimensional() {
    if (_dimLoaded) return;
    _dimLoaded = true;
    const container = root.querySelector('#dimensional-content');
    try {
      const chars = await api.characters.list({ inspection_id: id });
      _chars = chars;

      // Pre-populate _charState from existing results
      for (const c of chars) {
        const r = c.inspection_result;
        if (!r || r === 'Unidentified') {
          _charState[c.id] = { result: null, actual: '', note: c.note ?? '', zones: [] };
        } else if (r === 'Manual Accept' || r === 'Conform') {
          _charState[c.id] = { result: 'ok', actual: r, note: c.note ?? '', zones: [] };
        } else if (r === 'Manual Reject' || r === 'Not Conform') {
          _charState[c.id] = { result: 'fail', actual: r, note: c.note ?? '', zones: [] };
        } else {
          // min/max result like "17.16 / 17.88" — parse to determine ok/fail
          const allVals = r.split('/').map(v => parseFloat(v.trim())).filter(v => !isNaN(v));
          if (allVals.length > 0) {
            const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
            const ok = !hasLimits || allVals.every(v => v >= c.lower_limit && v <= c.upper_limit);
            _charState[c.id] = { result: ok ? 'ok' : 'fail', actual: r, note: c.note ?? '', zones: [] };
          } else {
            _charState[c.id] = { result: null, actual: '', note: c.note ?? '', zones: [] };
          }
        }
      }

      // Pre-populate _charDispState from existing dispositions (parallel fetch)
      // Also fetch categorical zone results for attribute (LOT) chars that have been measured
      const measuredLotChars = chars.filter(c => _isLot(c) && _charState[c.id]?.result !== null);
      const [dispFetches, zoneFetches] = await Promise.all([
        Promise.allSettled(chars.map(c => api.characters.listDispositions(c.id))),
        Promise.allSettled(measuredLotChars.map(c => api.zoneResults.list(c.id))),
      ]);
      dispFetches.forEach((r, i) => {
        _charDispState[chars[i].id] = r.status === 'fulfilled'
          ? (r.value.filter(d => d.decision !== 'VOID')
              .sort((a, b) => new Date(b.created_at) - new Date(a.created_at))[0] || null)
          : null;
      });
      zoneFetches.forEach((r, i) => {
        if (r.status === 'fulfilled' && r.value.length > 0) {
          _charState[measuredLotChars[i].id].zones =
            r.value.map(z => ({ zone_name: z.zone_name, is_confirmed: z.is_confirmed }));
        }
      });

      if (chars.length === 0) {
        container.innerHTML = `
          <div class="card">
            <div class="card-header">
              <span class="card-title">Dimensional Characteristics</span>
              ${pageCanWrite ? `
              <label class="btn btn-ghost btn-sm" for="dim-op-sheet-input">📁 Upload Op Sheet (.docx)</label>
              <input type="file" id="dim-op-sheet-input" class="file-input-hidden" accept=".docx" />` : ''}
            </div>
            <div class="empty">No op sheet uploaded — no characteristics found.</div>
          </div>`;
        root.querySelector('#dim-op-sheet-input')?.addEventListener('change', async (e) => {
          const file = e.target.files[0];
          if (!file) return;
          e.target.value = '';
          try {
            const result = await api.inspections.parseOpSheet(id, file);
            window.toast(`${result.characters_created} characteristics created.`, 'success');
            _dimLoaded = false;
            await loadDimensional();
          } catch (err) {
            window.toast('Parse error: ' + err.message, 'error');
          }
        });
        return;
      }

      // Build char list HTML
      const charListItems = chars.map((c, i) => {
        const disp = _charDispState[c.id];
        const dispType = disp ? (_dispTypeCache || []).find(t => t.code === disp.decision) : null;
        const dispBadge = disp
          ? `<span class="disp-badge disp-${dispType?.css_class || 'unknown'}" style="font-size:10px;padding:1px 5px;margin-left:4px">${disp.decision}</span>`
          : '';
        return `
        <div class="dim-char-item" data-index="${i}" data-id="${c.id}">
          <span class="dim-char-dot-wrap">${_charBadgeHtml(c)}</span>
          <span class="dim-char-text" style="min-width:0;flex:1">
            <span class="dim-char-no">${String(c.item_no).replace(/\s+/g, '')}</span>
            <span class="dim-char-dim">${c.dimension || ''}</span>
            ${dispBadge}
          </span>
          ${pageCanWrite ? `<button class="btn btn-ghost btn-xs dim-char-disp-btn" data-index="${i}"
                  title="Disposition" style="margin-left:4px;flex-shrink:0;">⚖</button>` : ''}
        </div>`;
      }).join('');

      container.innerHTML = `
        <div class="card" style="padding:0">
          <div class="card-header" style="padding:10px 16px">
            <span class="card-title">Dimensional Characteristics</span>
            ${pageCanWrite ? `
            <label class="btn btn-ghost btn-sm" for="dim-op-sheet-input-top">📁 Upload Op Sheet</label>
            <input type="file" id="dim-op-sheet-input-top" class="file-input-hidden" accept=".docx" />` : ''}
          </div>
          <div class="irs-stats-row" id="dim-stats-row" style="padding:8px 16px;border-bottom:1px solid var(--border)"></div>
          <div class="dim-split">
            <div class="dim-char-list">${charListItems}</div>
            <div class="dim-entry-panel" id="dim-entry-panel">
              <div class="dim-entry-placeholder">Select a characteristic to begin measurement.</div>
            </div>
            <div class="dim-doc-viewer" id="dim-doc-viewer">
              <div class="loading">Loading op sheet…</div>
            </div>
          </div>
        </div>`;

      _renderStats();

      // Char list click — activate in center panel only (use Detail button to open modal)
      root.querySelectorAll('.dim-char-item').forEach(li => {
        li.addEventListener('click', (e) => {
          if (e.target.closest('.dim-char-disp-btn')) return;
          _activateChar(Number(li.dataset.index));
        });
      });

      // Disposition button — open full disposition modal
      root.querySelectorAll('.dim-char-disp-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
          e.stopPropagation();
          const idx = Number(btn.dataset.index);
          _openCharacterDispositionModal(_chars[idx], async () => {
            _activateChar(idx);
          });
        });
      });

      // Op sheet upload from top button
      root.querySelector('#dim-op-sheet-input-top')?.addEventListener('change', async (e) => {
        const file = e.target.files[0];
        if (!file) return;
        e.target.value = '';
        try {
          const result = await api.inspections.parseOpSheet(id, file);
          window.toast(`${result.characters_created} characteristics created.`, 'success');
          _dimLoaded = false;
          await loadDimensional();
        } catch (err) {
          window.toast('Parse error: ' + err.message, 'error');
        }
      });

      // Keyboard navigation (ArrowLeft/Right) + Esc to close modal
      document.addEventListener('keydown', function _dimKeyNav(e) {
        if (!root.isConnected) { document.removeEventListener('keydown', _dimKeyNav); return; }
        if (e.key === 'Escape') { _closeDetailModal(); return; }
        const modalOpen = document.getElementById('dim-modal-overlay')
          && !document.getElementById('dim-modal-overlay').classList.contains('hidden');
        if (modalOpen) return;
        if (e.key === 'ArrowRight' && _activeIndex < _chars.length - 1) { e.preventDefault(); _activateChar(_activeIndex + 1); }
        if (e.key === 'ArrowLeft'  && _activeIndex > 0)                  { e.preventDefault(); _activateChar(_activeIndex - 1); }
      });

      // Activate first char + load doc viewer in parallel
      _activateChar(0);
      await _loadDocViewer();
      // Inject all saved actual values into the doc viewer
      for (const c of _chars) {
        const state = _charState[c.id];
        if (state?.actual && state.result !== null) {
          _updateDocActualCell(c, state.actual, state.result === 'ok');
        }
      }
      // Re-highlight after doc loaded
      if (_chars.length > 0) _highlightDocRow(_chars[_activeIndex]);

    } catch (err) {
      container.innerHTML = `<div class="empty text-danger">Load error: ${err.message}</div>`;
      _dimLoaded = false;
    }
  }

  root.querySelectorAll('.dual-tab').forEach(tab => {
    tab.addEventListener('click', async () => {
      root.querySelectorAll('.dual-tab').forEach(t => t.classList.remove('active'));
      tab.classList.add('active');
      const pane = tab.dataset.tab;
      root.querySelector('#tab-visual').style.display      = pane === 'visual'      ? '' : 'none';
      root.querySelector('#tab-dimensional').style.display = pane === 'dimensional' ? '' : 'none';
      if (pane === 'dimensional') await loadDimensional();
    });
  });

  // Op sheet upload handled inside loadDimensional() per-render

  // Rapor dropdown
  const reportDropBtn  = root.querySelector('#report-dropdown-btn');
  const reportDropMenu = root.querySelector('#report-dropdown-menu');
  reportDropBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    reportDropMenu.classList.toggle('open');
  });
  document.addEventListener('click', () => reportDropMenu.classList.remove('open'), { once: false });
  root.querySelector('#report-landscape-btn').addEventListener('click', () => {
    window.open(`/report.html?id=${id}&type=landscape`, '_blank');
  });
  root.querySelector('#report-detail-btn').addEventListener('click', () => {
    window.open(api.inspections.detailReportUrl(id), '_blank');
  });

  // Durum değiştir
  root.querySelector('#status-quick-change')?.addEventListener('change', async (e) => {
    const newStatus = e.target.value;
    if (newStatus === 'completed') {
      const types = await _loadDispTypes();
      const pending = detail.defects.filter(d =>
        !d.active_disposition || !types.some(t => t.code === d.active_disposition.decision && t.is_neutralizing)
      );
      if (pending.length > 0) {
        const ids = pending.map(d => '#' + d.id).join(', ');
        if (!confirm(`${pending.length} hata henüz sonuçlandırılmadı (${ids}).\nYine de tamamlandı olarak işaretlemek istiyor musunuz?`)) {
          e.target.value = detail.status; // revert select
          return;
        }
      }
    }
    try {
      await api.inspections.update(id, { status: newStatus });
      window.toast('Status updated.', 'success');
      await render(id, root);
    } catch (err) { window.toast('Hata: ' + err.message, 'error'); }
  });

  // Muayene sil
  root.querySelector('#delete-insp-btn')?.addEventListener('click', async () => {
    if (!confirm(`Are you sure you want to delete inspection #${id}?`)) return;
    try {
      await api.inspections.delete(id);
      window.toast('Inspection deleted.', 'success');
      window.navigate('/inspections');
    } catch (err) { window.toast('Silme hatası: ' + err.message, 'error'); }
  });

  // Hata ekle
  root.querySelector('#add-defect-btn')?.addEventListener('click', () => {
    openDefectModal(id, null, defectTypes, () => render(id, root));
  });

  // Hata düzenle / sil
  root.querySelectorAll('.edit-defect-btn').forEach(btn =>
    btn.addEventListener('click', () => {
      const defect = detail.defects.find(d => d.id === Number(btn.dataset.id));
      openDefectModal(id, defect, defectTypes, () => render(id, root));
    })
  );
  root.querySelectorAll('.delete-defect-btn').forEach(btn =>
    btn.addEventListener('click', async () => {
      const did = Number(btn.dataset.id);
      if (!confirm(`Hata #${did} silinecek. Emin misiniz?`)) return;
      try {
        await api.defects.delete(did);
        window.toast('Hata silindi.', 'success');
        await render(id, root);
      } catch (err) { window.toast('Silme hatası: ' + err.message, 'error'); }
    })
  );

  // Disposition düzelt (son kararı sil, yeniden gir)
  root.querySelectorAll('.disp-undo-btn').forEach(btn =>
    btn.addEventListener('click', async () => {
      const dispId = Number(btn.dataset.dispId);
      if (!confirm('Bu karar geri alınacak ve yeniden girilebilecek. Emin misiniz?')) return;
      try {
        await api.dispositions.delete(dispId);
        window.toast('Karar geri alındı.', 'success');
        await render(id, root);
      } catch (err) { window.toast('Hata: ' + err.message, 'error'); }
    })
  );

  // Disposition
  root.querySelectorAll('.disp-btn').forEach(btn =>
    btn.addEventListener('click', () => {
      const defect = detail.defects.find(d => d.id === Number(btn.dataset.id));
      if (defect.active_disposition?.decision === 'REWORK') {
        // 2-adımlı akış: önce post-rework ölçüm güncelleme, sonra doğrudan nihai karar
        openDefectModal(id, defect, defectTypes, async () => {
          const updated = await api.defects.get(defect.id);
          openDispositionModal(updated, () => render(id, root));
        }, null, true);
      } else {
        openDispositionModal(defect, () => render(id, root));
      }
    })
  );

  // Yeniden işleme sonucu yeni hata ekle
  root.querySelectorAll('.rework-child-btn').forEach(btn =>
    btn.addEventListener('click', () => {
      const originId = Number(btn.dataset.id);
      openDefectModal(id, null, defectTypes, () => render(id, root), originId);
    })
  );

  // Hataya fotoğraf ekle (+📷 butonları)
  root.querySelectorAll('.add-photo-to-defect').forEach(btn =>
    btn.addEventListener('click', () => {
      const defectId = Number(btn.dataset.defectId);
      openPhotoSourceModal(defectId, id, detail.defects, () => render(id, root));
    })
  );

  // Hata thumb'u — düzenle
  root.querySelectorAll('.thumb-ann-btn').forEach(btn =>
    btn.addEventListener('click', (e) => {
      e.stopPropagation();
      const pid = Number(btn.dataset.pid);
      const preselectedDefectIds = JSON.parse(btn.dataset.defects || '[]');
      openAnnotatorModal(pid, id, preselectedDefectIds, detail.defects, () => render(id, root));
    })
  );

  // Hata thumb'u — sil
  root.querySelectorAll('.thumb-del-btn').forEach(btn =>
    btn.addEventListener('click', async (e) => {
      e.stopPropagation();
      const pid = Number(btn.dataset.pid);
      if (!confirm('Fotoğraf silinsin mi?')) return;
      try {
        await api.photos.delete(pid);
        window.toast('Fotoğraf silindi.', 'success');
        await render(id, root);
      } catch (err) { window.toast('Silme hatası: ' + err.message, 'error'); }
    })
  );

  // Genel dosya yükleme — hata seç modal açılır
  root.querySelector('#photo-upload-input')?.addEventListener('change', async (e) => {
    const files = Array.from(e.target.files);
    if (!files.length) return;
    e.target.value = '';
    openUploadModal(id, [], detail.defects, () => render(id, root), files);
  });

  // Kamera
  root.querySelector('#open-camera-btn')?.addEventListener('click', () => {
    openCameraModal(id, detail.defects, () => render(id, root));
  });

  // Fotoğraf düzenle (annotator)
  root.querySelectorAll('.photo-annotate-btn').forEach(btn =>
    btn.addEventListener('click', () => {
      const pid = Number(btn.dataset.id);
      const preselectedDefectIds = JSON.parse(btn.dataset.defects || '[]');
      openAnnotatorModal(pid, id, preselectedDefectIds, detail.defects, () => render(id, root));
    })
  );

  // Fotoğraf sil
  root.querySelectorAll('.photo-delete-btn').forEach(btn =>
    btn.addEventListener('click', async () => {
      const pid = Number(btn.dataset.id);
      if (!confirm('Fotoğraf silinsin mi?')) return;
      try {
        await api.photos.delete(pid);
        window.toast('Fotoğraf silindi.', 'success');
        await render(id, root);
      } catch (err) { window.toast('Silme hatası: ' + err.message, 'error'); }
    })
  );
}

// ── Fotoğraf yükleme modal'ı (dosya seçimi veya önceden seçilmiş dosyalar) ──
function openUploadModal(inspectionId, preselectedDefectIds, defects, onDone, preFiles = null) {
  window.modal.open(`
    <div class="modal-card">
      <div class="modal-header">
        <span class="modal-title">📁 Fotoğraf Yükle</span>
        <button class="modal-close" id="upl-close">&times;</button>
      </div>

      <div class="form-group" style="margin-bottom:16px;" id="upl-defect-group">
        <label>Hangi hatalarla ilişkilendir?</label>
        ${defectCheckboxes(defects, preselectedDefectIds)}
      </div>

      <div class="form-group" id="upl-file-group" ${preFiles ? 'style="display:none"' : ''}>
        <label>Fotoğraf Seç</label>
        <input type="file" id="upl-file-input" class="form-input"
               accept="image/*" multiple />
      </div>

      <div id="upl-preview" class="upl-preview"></div>

      <div class="form-actions" style="margin-top:16px;">
        <button class="btn btn-ghost"   id="upl-cancel">İptal</button>
        <button class="btn btn-primary" id="upl-submit" ${preFiles ? '' : 'disabled'}>
          Yükle
        </button>
      </div>
    </div>
  `);

  let filesToUpload = preFiles || [];

  // Önceden seçilmiş dosyalar varsa önizleme göster
  if (preFiles) renderPreview(preFiles);

  document.getElementById('upl-close').addEventListener('click', () => window.modal.close());
  document.getElementById('upl-cancel').addEventListener('click', () => window.modal.close());

  document.getElementById('upl-file-input')?.addEventListener('change', (e) => {
    filesToUpload = Array.from(e.target.files);
    document.getElementById('upl-submit').disabled = filesToUpload.length === 0;
    renderPreview(filesToUpload);
  });

  document.getElementById('upl-submit').addEventListener('click', async () => {
    if (!filesToUpload.length) return;
    const defect_ids = getCheckedDefectIds('upl-defect-group');
    const btn = document.getElementById('upl-submit');
    btn.disabled = true;
    btn.textContent = 'Yükleniyor...';
    try {
      for (const file of filesToUpload) {
        await api.photos.upload(inspectionId, file, defect_ids);
      }
      window.toast(`${filesToUpload.length} fotoğraf yüklendi.`, 'success');
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Yükleme hatası: ' + err.message, 'error');
      btn.disabled = false;
      btn.textContent = 'Yükle';
    }
  });

  function renderPreview(files) {
    const el = document.getElementById('upl-preview');
    el.innerHTML = files.map(f =>
      `<div class="upl-thumb-item">
        <img src="${URL.createObjectURL(f)}" class="upl-thumb" />
        <span class="text-secondary" style="font-size:11px;">${f.name}</span>
       </div>`
    ).join('');
  }
}

// ── Fotoğraf kaynak seçici (defect satırındaki +📷 butonu) ───────────────────
function openPhotoSourceModal(defectId, inspectionId, defects, onDone) {
  window.modal.open(`
    <div class="modal-card" style="max-width:340px;">
      <div class="modal-header">
        <span class="modal-title">📷 Fotoğraf Ekle — Hata #${defectId}</span>
        <button class="modal-close" id="src-close">&times;</button>
      </div>
      <p class="text-secondary" style="font-size:13px;margin:12px 0 20px;">
        Fotoğraf kaynağını seçin:
      </p>
      <div style="display:flex;flex-direction:column;gap:10px;">
        <button class="btn btn-primary" id="src-camera">
          📷 &nbsp;Kamera ile Çek
        </button>
        <button class="btn btn-ghost" id="src-file">
          📁 &nbsp;Dosya Yükle
        </button>
      </div>
    </div>
  `);

  document.getElementById('src-close').addEventListener('click', () => window.modal.close());

  document.getElementById('src-camera').addEventListener('click', () => {
    window.modal.close();
    // Small delay so the previous modal fully tears down before opening the next
    setTimeout(() => openCameraModal(inspectionId, defects, onDone, [defectId]), 80);
  });

  document.getElementById('src-file').addEventListener('click', () => {
    window.modal.close();
    setTimeout(() => openUploadModal(inspectionId, [defectId], defects, onDone), 80);
  });
}

// ── Kamera modal'ı ───────────────────────────────────────────────────────────
function openCameraModal(inspectionId, defects, onDone, preselectedIds = []) {
  let capturedBlob = null;
  const cam = new Camera();

  window.modal.open(`
    <div class="modal-card modal-camera">
      <div class="modal-header">
        <span class="modal-title">📷 Fotoğraf Çek</span>
        <button class="modal-close" id="cam-close">&times;</button>
      </div>
      <div id="camera-container"></div>

      <div id="cam-review" class="cam-review hidden">
        <img id="cam-review-img" style="width:100%;border-radius:8px;margin-top:12px;" />
        <div class="form-group" style="margin-top:12px;" id="cam-defect-group">
          <label>Hangi hatalarla ilişkilendir?</label>
          ${defectCheckboxes(defects, preselectedIds)}
        </div>
      </div>

      <div class="form-actions" style="margin-top:14px;">
        <button class="btn btn-ghost"   id="cam-cancel">Kapat</button>
        <button class="btn btn-ghost"   id="cam-retake"  style="display:none">↺ Tekrar Çek</button>
        <button class="btn btn-primary" id="cam-save"    style="display:none">💾 Kaydet</button>
      </div>
    </div>
  `, {
    wide: true,
    onClose: () => cam.close(),
  });

  cam.open(document.getElementById('camera-container'));

  cam.on('photo', (blob) => {
    capturedBlob = blob;
    const url = URL.createObjectURL(blob);
    document.getElementById('cam-review-img').src = url;
    document.getElementById('cam-review').classList.remove('hidden');
    document.getElementById('cam-retake').style.display = '';
    document.getElementById('cam-save').style.display   = '';
    document.getElementById('cam-cancel').style.display = 'none';
  });

  document.getElementById('cam-close').addEventListener('click', () => window.modal.close());
  document.getElementById('cam-cancel').addEventListener('click', () => window.modal.close());

  document.getElementById('cam-retake').addEventListener('click', () => {
    capturedBlob = null;
    document.getElementById('cam-review').classList.add('hidden');
    document.getElementById('cam-retake').style.display = 'none';
    document.getElementById('cam-save').style.display   = 'none';
    document.getElementById('cam-cancel').style.display = '';
  });

  document.getElementById('cam-save').addEventListener('click', async () => {
    if (!capturedBlob) return;
    const defect_ids = getCheckedDefectIds('cam-defect-group');
    // Sunucuya YÜKLEME YOK — blob doğrudan annotator'a gidiyor
    // Annotator'da "Kaydet" denince tek seferinde yüklenecek
    window.modal.close();
    await new Promise(r => setTimeout(r, 80));
    await openAnnotatorFromBlob(
      capturedBlob,
      inspectionId,
      defect_ids,
      defects,
      onDone
    );
  });
}

// ── Annotator — kamera blob'undan (henüz yüklenmemiş) ────────────────────────
// Kullanıcı "Kaydet" dediğinde TEK fotoğraf yüklenir.
async function openAnnotatorFromBlob(blob, inspectionId, defect_ids, defects, onDone) {
  const localUrl = URL.createObjectURL(blob);
  const ann = new Annotator();

  window.modal.open(`
    <div class="modal-card modal-annotator">
      <div class="modal-header">
        <span class="modal-title">✏ Fotoğrafı Düzenle</span>
        <button class="modal-close" id="ann-close">&times;</button>
      </div>
      <div id="ann-mount"></div>
      <div class="ann-footer">
        <div class="form-group" style="margin:0;min-width:220px;" id="ann-defect-group">
          <label style="font-size:12px;margin-bottom:4px;">Hata ile ilişkilendir</label>
          ${defectCheckboxes(defects, defect_ids)}
        </div>
        <div class="ann-footer-actions">
          <button class="btn btn-ghost btn-sm"   id="ann-cancel-btn">İptal (kaydetme)</button>
          <button class="btn btn-primary btn-sm" id="ann-save-btn">💾 Kaydet</button>
        </div>
      </div>
    </div>
  `, {
    wide: true,
    onClose: () => { ann.destroy(); URL.revokeObjectURL(localUrl); },
  });

  try {
    await ann.open(document.getElementById('ann-mount'), localUrl);
  } catch (err) {
    window.toast('Görüntü yüklenemedi: ' + err.message, 'error');
    window.modal.close();
    return;
  }

  document.getElementById('ann-close').addEventListener('click',      () => window.modal.close());
  document.getElementById('ann-cancel-btn').addEventListener('click', () => window.modal.close());

  document.getElementById('ann-save-btn').addEventListener('click', async () => {
    const btn        = document.getElementById('ann-save-btn');
    const checkedIds = getCheckedDefectIds('ann-defect-group');
    btn.disabled     = true;
    btn.textContent  = 'Kaydediliyor...';
    try {
      const label    = checkedIds.length ? checkedIds.map(id => `#${id}`).join(' ') : null;
      const exported = await ann.exportBlobWithLabel(label);
      const file     = new File([exported], `cam_${Date.now()}.jpg`, { type: 'image/jpeg' });
      await api.photos.upload(inspectionId, file, checkedIds);
      window.toast('Fotoğraf kaydedildi.', 'success');
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Kayıt hatası: ' + err.message, 'error');
      btn.disabled    = false;
      btn.textContent = '💾 Kaydet';
    }
  });
}

// ── Annotator modal'ı — mevcut sunucu fotoğrafını düzenle ────────────────────
async function openAnnotatorModal(photoId, inspectionId, currentDefectIds, defects, onDone) {
  const imageUrl = api.photos.fileUrl(photoId);
  const ann = new Annotator();

  window.modal.open(`
    <div class="modal-card modal-annotator">
      <div class="modal-header">
        <span class="modal-title">✏ Fotoğraf Düzenle — #${photoId}</span>
        <button class="modal-close" id="ann-close">&times;</button>
      </div>
      <div id="ann-mount"></div>
      <div class="ann-footer">
        <div class="form-group" style="margin:0;min-width:220px;" id="ann-defect-group">
          <label style="font-size:12px;margin-bottom:4px;">Hata ile ilişkilendir</label>
          ${defectCheckboxes(defects, currentDefectIds)}
        </div>
        <div class="ann-footer-actions">
          <button class="btn btn-ghost btn-sm"   id="ann-cancel-btn">İptal</button>
          <button class="btn btn-primary btn-sm" id="ann-save-btn">💾 Kaydet</button>
        </div>
      </div>
    </div>
  `, { wide: true, onClose: () => ann.destroy() });

  try {
    await ann.open(document.getElementById('ann-mount'), imageUrl);
  } catch {
    window.toast('Fotoğraf yüklenemedi.', 'error');
    window.modal.close();
    return;
  }

  document.getElementById('ann-close').addEventListener('click',      () => window.modal.close());
  document.getElementById('ann-cancel-btn').addEventListener('click', () => window.modal.close());

  document.getElementById('ann-save-btn').addEventListener('click', async () => {
    const btn        = document.getElementById('ann-save-btn');
    const checkedIds = getCheckedDefectIds('ann-defect-group');
    btn.disabled     = true;
    btn.textContent  = 'Kaydediliyor...';
    try {
      // Stamp defect ID badge onto a copy; live canvas stays clean for reuse
      const label = checkedIds.length ? checkedIds.map(id => `#${id}`).join(' ') : null;
      const blob  = await ann.exportBlobWithLabel(label);
      const file  = new File([blob], `annotated_${Date.now()}.jpg`, { type: 'image/jpeg' });
      await api.photos.upload(inspectionId, file, checkedIds);
      // Only delete original when editing an already-linked defect photo.
      // General photos (currentDefectIds empty) are kept so they can be
      // reused to annotate other defects without re-photographing.
      if (currentDefectIds.length > 0) {
        await api.photos.delete(photoId);
      }
      window.toast('Fotoğraf güncellendi.', 'success');
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Kayıt hatası: ' + err.message, 'error');
      btn.disabled    = false;
      btn.textContent = '💾 Kaydet';
    }
  });
}

// ── Disposition modal'ı ──────────────────────────────────────────────────────
async function openDispositionModal(defect, onDone) {
  const active          = defect.active_disposition;
  const currentDecision = active?.decision || null;
  const today           = new Date().toISOString().split('T')[0];

  // Build allowed decision options from API (already cached after render())
  const allowed = await allowedNextDecisions(currentDecision);
  const types   = await _loadDispTypes();
  const decisionOptions = [
    { value: '', label: '— Karar Seçiniz —' },
    ...types
      .filter(t => t.active && (allowed.length === 0 || allowed.includes(t.code)))
      .sort((a, b) => a.sort_order - b.sort_order)
      .map(t => ({ value: t.code, label: t.label })),
  ];

  window.modal.open(`
    <div class="modal-card">
      <div class="modal-header">
        <span class="modal-title">⚖ Disposition — Hata #${defect.id}</span>
        <button class="modal-close" id="dp-close">&times;</button>
      </div>

      ${active ? `
        <div class="disp-history-box">
          ${dispositionBadge(active)}
          <span class="disp-note-text">${active.note}</span>
        </div>` : ''}

      <form id="dp-form" novalidate>

        <!-- Karar -->
        <div class="form-group" style="margin-bottom:14px;">
          <label>Karar *</label>
          <select class="form-select" name="decision" id="dp-decision" required>
            ${decisionOptions.map(o => `<option value="${o.value}">${o.label}</option>`).join('')}
          </select>
        </div>

        <!-- Ortak alanlar: mühendis + sicil no + tarih -->
        <div class="form-grid" style="margin-bottom:14px;">
          <div class="form-group" id="dp-engineer-group">
            <label>Kararı Veren Mühendis</label>
            <input type="text" class="form-input" name="engineer"
                   placeholder="Mühendis adı soyadı" />
          </div>
          <div class="form-group">
            <label>Sicil No (Sisteme Giren Inspector) *</label>
            <input type="text" class="form-input" name="entered_by"
                   placeholder="Inspector sicil no"
                   value="${session.get() || ''}" />
          </div>
        </div>
        <div class="form-group" style="margin-bottom:14px;">
          <label>Karar Tarihi *</label>
          <input type="date" class="form-input" name="decided_at"
                 value="${today}" required />
        </div>

        <!-- USE_AS_IS alanları -->
        <div id="dp-fields-USE_AS_IS" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>Spec / Doküman No</label>
            <input type="text" class="form-input" name="spec_ref" placeholder="ör. ENG-SPEC-4421" />
          </div>
        </div>

        <!-- KABUL_RESIM alanları -->
        <div id="dp-fields-KABUL_RESIM" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>Resim / Çizim No</label>
            <input type="text" class="form-input" name="spec_ref" placeholder="ör. DRW-2026-0042" />
          </div>
        </div>

        <!-- CONFORMS alanları -->
        <div id="dp-fields-CONFORMS" class="dp-extra-fields" style="display:none;">
          <p class="text-secondary" style="font-size:12px;padding:4px 0;">
            Hata giderildi veya ölçüm sınır içinde. Inspector onayı yeterlidir, mühendis kararı gerekmez.
          </p>
        </div>

        <!-- REWORK alanları -->
        <div id="dp-fields-REWORK" class="dp-extra-fields" style="display:none;">
          <p class="text-secondary" style="font-size:12px;padding:4px 0;">
            Parça yeniden işleme gönderildi. İşlem sonrası yeniden inceleme yapılacak.
          </p>
        </div>

        <!-- RE_INSPECT alanları -->
        <div id="dp-fields-RE_INSPECT" class="dp-extra-fields" style="display:none;">
          <p class="text-secondary" style="font-size:12px;padding:4px 0;">
            Part sent for re-inspection. Decision will be entered based on inspection result.
          </p>
        </div>

        <!-- CTP_RE_INSPECT alanları -->
        <div id="dp-fields-CTP_RE_INSPECT" class="dp-extra-fields" style="display:none;">
          <p class="text-secondary" style="font-size:12px;padding:4px 0;">
            Parça bir sonraki operasyona devam ediyor; o operasyonda yeniden inceleme zorunludur.
          </p>
          <div class="form-group" style="margin-bottom:14px;">
            <label>Sonraki Operasyon No <span style="color:#888;font-weight:400;">(opsiyonel)</span></label>
            <input type="text" class="form-input" name="ctp_ri_op" placeholder="ör. OP-020" />
          </div>
        </div>

        <!-- MRB_SUBMITTED alanları -->
        <div id="dp-fields-MRB_SUBMITTED" class="dp-extra-fields" style="display:none;">
          <p class="text-secondary" style="font-size:12px;padding:4px 0;">
            NCR MRB'ye iletildi.
          </p>
        </div>

        <!-- MRB_CTP alanları -->
        <div id="dp-fields-MRB_CTP" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>CTP Referans No (isteğe bağlı)</label>
            <input type="text" class="form-input" name="ctp_ref" placeholder="ör. CTP-2026-007" />
          </div>
        </div>

        <!-- MRB_ACCEPTED alanları -->
        <div id="dp-fields-MRB_ACCEPTED" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>Concession / Case Record No</label>
            <input type="text" class="form-input" name="concession_no" placeholder="ör. MRB-2026-001" />
          </div>
        </div>

        <!-- MRB_REJECTED alanları -->
        <div id="dp-fields-MRB_REJECTED" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>NCR / Ret Referans No (isteğe bağlı)</label>
            <input type="text" class="form-input" name="mrb_reject_ref" placeholder="ör. NCR-2026-042" />
          </div>
        </div>

        <!-- VOID alanları -->
        <div id="dp-fields-VOID" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>Geçersizlik Gerekçesi</label>
            <input type="text" class="form-input" name="void_reason" placeholder="Neden geçersiz?" />
          </div>
        </div>

        <!-- REPAIR alanları -->
        <div id="dp-fields-REPAIR" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>Onarım Prosedür / Doküman No</label>
            <input type="text" class="form-input" name="repair_ref" placeholder="ör. REPAIR-PROC-123" />
          </div>
        </div>

        <!-- SCRAP alanları -->
        <div id="dp-fields-SCRAP" class="dp-extra-fields" style="display:none;">
          <div class="form-group" style="margin-bottom:14px;">
            <label>Hurda Gerekçesi</label>
            <input type="text" class="form-input" name="scrap_reason" placeholder="Hurda nedeni" />
          </div>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-ghost" id="dp-cancel">İptal</button>
          <button type="submit" class="btn btn-primary" id="dp-submit">Kaydet</button>
        </div>
      </form>
    </div>
  `);

  document.getElementById('dp-close').addEventListener('click',  () => window.modal.close());
  document.getElementById('dp-cancel').addEventListener('click', () => window.modal.close());

  // Karar seçilince ilgili alanları göster
  const decisionSel  = document.getElementById('dp-decision');
  const engineerGroup = document.getElementById('dp-engineer-group');
  function showFields(val) {
    document.querySelectorAll('.dp-extra-fields').forEach(el => el.style.display = 'none');
    if (val) {
      const el = document.getElementById(`dp-fields-${val}`);
      if (el) el.style.display = '';
    }
    // CONFORMS = inspector-only, mühendis alanı gizlenir
    if (engineerGroup) engineerGroup.style.display = (val === 'CONFORMS') ? 'none' : '';
  }
  decisionSel.addEventListener('change', () => showFields(decisionSel.value));

  document.getElementById('dp-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn       = document.getElementById('dp-submit');
    const fd        = new FormData(e.target);
    const decision   = fd.get('decision');
    const entered_by = (fd.get('entered_by') || '').trim();
    const engineer   = (fd.get('engineer')   || '').trim();
    const decided_at = fd.get('decided_at');

    if (!decision)   { window.toast('Karar seçiniz.', 'error'); return; }
    if (!entered_by) { window.toast('Sicil no zorunludur.', 'error'); return; }

    const payload = { defect_id: defect.id, decision, entered_by, decided_at };
    if (engineer) payload.engineer = engineer;

    if (decision === 'USE_AS_IS' || decision === 'KABUL_RESIM') {
      const ref = (fd.get('spec_ref') || '').trim();
      if (ref) payload.spec_ref = ref;
    } else if (decision === 'CTP_RE_INSPECT') {
      const op = (fd.get('ctp_ri_op') || '').trim();
      if (op) payload.spec_ref = op;
    } else if (decision === 'MRB_CTP') {
      const ref = (fd.get('ctp_ref') || '').trim();
      if (ref) payload.spec_ref = ref;
    } else if (decision === 'MRB_ACCEPTED') {
      const ref = (fd.get('concession_no') || '').trim();
      if (ref) payload.concession_no = ref;
    } else if (decision === 'MRB_REJECTED') {
      const ref = (fd.get('mrb_reject_ref') || '').trim();
      if (ref) payload.concession_no = ref;
    } else if (decision === 'VOID') {
      const ref = (fd.get('void_reason') || '').trim();
      if (ref) payload.void_reason = ref;
    } else if (decision === 'REPAIR') {
      const ref = (fd.get('repair_ref') || '').trim();
      if (ref) payload.repair_ref = ref;
    } else if (decision === 'SCRAP') {
      const ref = (fd.get('scrap_reason') || '').trim();
      if (ref) payload.scrap_reason = ref;
    }

    btn.disabled = true;
    btn.textContent = 'Kaydediliyor...';
    try {
      await api.dispositions.create(payload);
      window.toast('Disposition kaydedildi.', 'success');
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Hata: ' + err.message, 'error');
      btn.disabled = false;
      btn.textContent = 'Kaydet';
    }
  });
}

// ── Hata modal'ı ─────────────────────────────────────────────────────────────
function openDefectModal(inspectionId, existingDefect, defectTypes, onDone, originDefectId = null, postRework = false) {
  const isEdit = !!existingDefect;
  const v      = existingDefect || {};

  function renderDynamicFields(fields) {
    return fields.map(f => {
      const reqMark = f.required ? ' <span style="color:#dc3545;">*</span>' : '';
      const val = v[f.field_name] ?? '';
      if (f.field_type === 'number') {
        return `
          <div class="form-group" data-field="${f.field_name}">
            <label>${f.label}${f.unit ? ` (${f.unit})` : ''}${reqMark}</label>
            <input type="number" step="0.01" class="form-input" name="${f.field_name}"
                   value="${val}" placeholder="0.00"
                   ${f.required ? 'data-required="1"' : ''} />
          </div>`;
      } else {
        return `
          <div class="form-group" data-field="${f.field_name}">
            <label>${f.label}${reqMark}</label>
            <input type="text" class="form-input" name="${f.field_name}"
                   value="${val}" placeholder=""
                   ${f.required ? 'data-required="1"' : ''} />
          </div>`;
      }
    }).join('');
  }

  const initialTypeId = v.defect_type_id || '';

  window.modal.open(`
    <div class="modal-card">
      <div class="modal-header">
        <span class="modal-title">${isEdit ? `Edit Defect #${v.id}` : 'Add Defect'}</span>
        <button class="modal-close" id="df-close">&times;</button>
      </div>
      ${postRework ? `
        <div class="origin-banner" style="background:#fff3cd;color:#856404;border-left:4px solid #ffc107;padding:10px 14px;margin-bottom:12px;border-radius:4px;font-size:13px;">
          📐 <strong>Post-rework ölçümleri</strong> — Rework sonrası ölçüm değerlerini güncelleyin.
          Kaydettikten sonra RE_INSPECT karar formu otomatik açılacak.
        </div>` : ''}
      ${originDefectId ? `
        <div class="origin-banner">
          ↳ Yeniden işleme sonucu — Hata #${originDefectId} kaynaklı yeni hata
        </div>` : ''}
      <form id="defect-form" novalidate>
        <div class="form-grid">
          <div class="form-group form-group-full">
            <label>Hata Tipi *</label>
            <select class="form-select" id="df-type-select" name="defect_type_id" required>
              <option value="">— Seçiniz —</option>
              ${defectTypes.map(dt =>
                `<option value="${dt.id}" ${v.defect_type_id == dt.id ? 'selected' : ''}>${dt.name}</option>`
              ).join('')}
            </select>
          </div>
          <div id="df-dynamic-fields" class="form-group-full" style="display:contents;">
            <div class="text-secondary" style="font-size:13px;padding:4px;">Hata tipi seçiniz...</div>
          </div>
          <div class="form-group form-group-full">
            <label>Notlar</label>
            <textarea class="form-textarea" name="notes" rows="3"
                      placeholder="Hata açıklaması...">${v.notes || ''}</textarea>
          </div>
          <div class="form-group form-group-full">
            <label class="checkbox-label" style="display:flex;align-items:center;gap:8px;cursor:pointer;">
              <input type="checkbox" name="high_metal" value="1" ${v.high_metal ? 'checked' : ''}
                     style="width:18px;height:18px;accent-color:#dc3545;" />
              <span>
                <strong>High Metal</strong>
                <span style="color:#6c757d;font-size:12px;margin-left:6px;">— Çevreleyen yüzeyden yüksek, FOD riski</span>
              </span>
            </label>
          </div>
        </div>
        <div id="df-validation-error" style="display:none;color:#dc3545;font-size:13px;margin-bottom:8px;padding:6px 10px;background:#fff5f5;border-radius:4px;border-left:3px solid #dc3545;"></div>
        <div class="form-actions">
          <button type="button" class="btn btn-ghost" id="df-cancel">İptal</button>
          <button type="submit" class="btn btn-primary" id="df-submit">
            ${isEdit ? 'Kaydet' : 'Ekle'}
          </button>
        </div>
      </form>
    </div>
  `);

  document.getElementById('df-close').addEventListener('click',  () => window.modal.close());
  document.getElementById('df-cancel').addEventListener('click', () => window.modal.close());

  const typeSelect       = document.getElementById('df-type-select');
  const dynamicContainer = document.getElementById('df-dynamic-fields');
  let currentFields      = [];

  async function loadFields(typeId) {
    if (!typeId) {
      dynamicContainer.innerHTML = '<div class="text-secondary" style="font-size:13px;padding:4px;">Hata tipi seçiniz...</div>';
      currentFields = [];
      return;
    }
    dynamicContainer.innerHTML = '<div class="text-secondary" style="font-size:13px;">Yükleniyor...</div>';
    currentFields = await getDefectFields(Number(typeId));
    dynamicContainer.innerHTML = renderDynamicFields(currentFields);
  }

  typeSelect.addEventListener('change', () => loadFields(typeSelect.value));

  // Load fields for initial value
  if (initialTypeId) {
    loadFields(initialTypeId);
  } else {
    dynamicContainer.innerHTML = '<div class="text-secondary" style="font-size:13px;padding:4px;">Hata tipi seçiniz...</div>';
  }

  document.getElementById('defect-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn = document.getElementById('df-submit');
    const errEl = document.getElementById('df-validation-error');
    errEl.style.display = 'none';

    const fd  = new FormData(e.target);
    const typeId = Number(fd.get('defect_type_id'));

    if (!typeId) {
      window.toast('Lütfen hata tipi seçiniz.', 'error');
      return;
    }

    // Required field validation
    for (const f of currentFields) {
      if (f.required) {
        const val = (fd.get(f.field_name) || '').trim();
        if (!val) {
          errEl.textContent = `"${f.label}" alanı zorunludur.`;
          errEl.style.display = 'block';
          return;
        }
      }
    }

    btn.disabled = true;
    btn.textContent = 'Kaydediliyor...';

    const data = {
      inspection_id:  inspectionId,
      defect_type_id: typeId,
      notes:          fd.get('notes') || null,
      high_metal:     fd.get('high_metal') === '1',
      ...(originDefectId ? { origin_defect_id: originDefectId } : {}),
    };

    // Collect dynamic field values
    for (const f of currentFields) {
      const raw = fd.get(f.field_name);
      if (raw !== null && raw !== '') {
        data[f.field_name] = f.field_type === 'number' ? parseFloat(raw) : raw;
      } else {
        data[f.field_name] = null;
      }
    }

    try {
      if (isEdit) await api.defects.update(v.id, data);
      else        await api.defects.create(data);
      window.toast(isEdit ? 'Hata güncellendi.' : 'Hata eklendi.', 'success');
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Hata: ' + err.message, 'error');
      btn.disabled = false;
      btn.textContent = isEdit ? 'Kaydet' : 'Ekle';
    }
  });
}

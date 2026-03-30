import { api } from '../api.js';
import { navigate } from '../router.js';

export async function characterDetailPage(projectId, charId) {
  const root = document.getElementById('page-root');
  root.innerHTML = `<div class="irs-loading">Yükleniyor…</div>`;

  try {
    const [char, numeric, categorical, zone] = await Promise.all([
      api.characters.get(charId),
      api.numericResults.list(charId),
      api.categoricalResults.list(charId),
      api.zoneResults.list(charId),
    ]);

    renderPage(root, char, numeric, categorical, zone, projectId);
  } catch (err) {
    root.innerHTML = `<p class="text-danger">Yüklenemedi: ${err.message}</p>`;
  }
}

function renderPage(root, char, numeric, categorical, zone, projectId) {
  const hasLimits = char.lower_limit !== 0 || char.upper_limit !== 0;
  const resultMeta = getResultMeta(char.inspection_result);

  // Determine measurement type from existing data
  const hasCategorical = categorical.length > 0 || zone.length > 0;
  const defaultTab = hasCategorical ? 'categorical' : 'numeric';

  root.innerHTML = `
    <div class="page-header">
      <div>
        <div class="breadcrumb">
          <a href="#/irs" class="breadcrumb-link">IRS Projeleri</a>
          <span class="breadcrumb-sep">›</span>
          <a href="#/irs/${projectId}" class="breadcrumb-link">Proje</a>
          <span class="breadcrumb-sep">›</span>
          <span>Item ${char.item_no}</span>
        </div>
        <h1 class="page-title">Item ${char.item_no}</h1>
        <p class="page-subtitle">${char.dimension}${char.bp_zone ? ' · Zone ' + char.bp_zone : ''}${char.badge ? ' · ' + char.badge : ''}</p>
      </div>
      <span class="result-badge result-badge-lg ${resultMeta.cls}">${resultMeta.label}</span>
    </div>

    ${hasLimits ? renderToleranceGauge(char, numeric) : ''}

    <div class="char-info-row">
      ${infoChip('Alt Sınır', hasLimits ? char.lower_limit.toFixed(4) : '—')}
      ${infoChip('Üst Sınır', hasLimits ? char.upper_limit.toFixed(4) : '—')}
      ${infoChip('Ölçüm Sayısı', numeric.length + categorical.length + zone.length)}
      ${char.inspection_level ? infoChip('Seviye', char.inspection_level) : ''}
      ${char.tooling ? infoChip('Tooling', char.tooling) : ''}
      ${char.remarks ? infoChip('Not', char.remarks) : ''}
    </div>

    <div class="irs-table-card">
      <div class="char-tab-bar">
        <button class="char-tab ${defaultTab === 'numeric' ? 'active' : ''}" data-tab="numeric">
          Sayısal <span class="tab-count">${numeric.length}</span>
        </button>
        <button class="char-tab ${defaultTab === 'categorical' ? 'active' : ''}" data-tab="categorical">
          Kategorik <span class="tab-count">${categorical.length}</span>
        </button>
        <button class="char-tab" data-tab="zone">
          Bölge <span class="tab-count">${zone.length}</span>
        </button>
        <div class="char-tab-spacer"></div>
        <button class="btn btn-primary btn-sm" id="btn-add-meas">+ Ölçüm Ekle</button>
      </div>

      <div id="tab-numeric"  class="tab-panel ${defaultTab === 'numeric' ? '' : 'hidden'}">
        ${renderNumericTable(numeric, char)}
      </div>
      <div id="tab-categorical" class="tab-panel ${defaultTab === 'categorical' ? '' : 'hidden'}">
        ${renderCategoricalTable(categorical)}
      </div>
      <div id="tab-zone" class="tab-panel hidden">
        ${renderZoneTable(zone)}
      </div>
    </div>`;

  // Tab switching
  root.querySelectorAll('.char-tab').forEach(btn => {
    btn.addEventListener('click', () => {
      root.querySelectorAll('.char-tab').forEach(b => b.classList.remove('active'));
      root.querySelectorAll('.tab-panel').forEach(p => p.classList.add('hidden'));
      btn.classList.add('active');
      document.getElementById(`tab-${btn.dataset.tab}`).classList.remove('hidden');
    });
  });

  document.getElementById('btn-add-meas').addEventListener('click', () => {
    const activeTab = root.querySelector('.char-tab.active')?.dataset.tab || 'numeric';
    openMeasurementModal(char, activeTab, async () => {
      const [n2, c2, z2] = await Promise.all([
        api.numericResults.list(char.id),
        api.categoricalResults.list(char.id),
        api.zoneResults.list(char.id),
      ]);
      // Update inspection_result based on numeric measurements
      if (n2.length > 0 && (char.lower_limit !== 0 || char.upper_limit !== 0)) {
        const allOk = n2.every(r => r.actual >= char.lower_limit && r.actual <= char.upper_limit);
        const newResult = allOk ? 'WithinTolerance' : 'OutOfTolerance';
        if (newResult !== char.inspection_result) {
          await api.characters.update(char.id, {
            item_no: char.item_no, dimension: char.dimension,
            badge: char.badge, bp_zone: char.bp_zone,
            tooling: char.tooling, inspection_level: char.inspection_level,
            remarks: char.remarks, irs_project_id: char.irs_project_id,
            inspection_result: newResult,
          }).catch(() => {});
        }
      }
      characterDetailPage(projectId, char.id);
    });
  });

  // Delete buttons
  root.querySelectorAll('[data-del-numeric]').forEach(btn => {
    btn.addEventListener('click', async () => {
      if (!confirm('Bu ölçüm silinsin mi?')) return;
      await api.numericResults.delete(btn.dataset.delNumeric).catch(e => toast(e.message, 'error'));
      characterDetailPage(projectId, char.id);
    });
  });
  root.querySelectorAll('[data-del-cat]').forEach(btn => {
    btn.addEventListener('click', async () => {
      if (!confirm('Bu ölçüm silinsin mi?')) return;
      await api.categoricalResults.delete(btn.dataset.delCat).catch(e => toast(e.message, 'error'));
      characterDetailPage(projectId, char.id);
    });
  });
  root.querySelectorAll('[data-del-zone]').forEach(btn => {
    btn.addEventListener('click', async () => {
      if (!confirm('Bu ölçüm silinsin mi?')) return;
      await api.zoneResults.delete(btn.dataset.delZone).catch(e => toast(e.message, 'error'));
      characterDetailPage(projectId, char.id);
    });
  });
}

function renderToleranceGauge(char, numeric) {
  const lo = char.lower_limit, hi = char.upper_limit;
  const spread = hi - lo;
  const margin = spread * 0.25;
  const viewLo = lo - margin, viewHi = hi + margin;
  const viewRange = viewHi - viewLo;

  const toPercent = v => Math.max(0, Math.min(100, ((v - viewLo) / viewRange) * 100));
  const rangeLeft = toPercent(lo);
  const rangeWidth = toPercent(hi) - rangeLeft;

  const dots = numeric.map(r => {
    const x = toPercent(r.actual);
    const ok = r.actual >= lo && r.actual <= hi;
    return `<div class="gauge-dot ${ok ? 'gauge-dot-ok' : 'gauge-dot-fail'}"
                 style="left:${x}%"
                 title="${r.actual}"></div>`;
  }).join('');

  const mean = numeric.length
    ? numeric.reduce((s, r) => s + r.actual, 0) / numeric.length
    : null;
  const meanMark = mean !== null
    ? `<div class="gauge-mean" style="left:${toPercent(mean)}%" title="Ort: ${mean.toFixed(4)}"></div>`
    : '';

  return `
    <div class="tolerance-gauge-wrap">
      <div class="gauge-labels">
        <span>${viewLo.toFixed(3)}</span>
        <span class="gauge-label-lo">${lo.toFixed(4)}</span>
        <span class="gauge-label-hi">${hi.toFixed(4)}</span>
        <span>${viewHi.toFixed(3)}</span>
      </div>
      <div class="tolerance-gauge">
        <div class="gauge-track"></div>
        <div class="gauge-range" style="left:${rangeLeft}%;width:${rangeWidth}%"></div>
        ${meanMark}
        ${dots}
        <div class="gauge-limit-line" style="left:${rangeLeft}%"></div>
        <div class="gauge-limit-line" style="left:${rangeLeft + rangeWidth}%"></div>
      </div>
      ${numeric.length > 0 ? `<p class="gauge-hint">${numeric.length} ölçüm · Ort: ${mean.toFixed(4)}</p>` : '<p class="gauge-hint">Henüz ölçüm yok</p>'}
    </div>`;
}

function renderNumericTable(rows, char) {
  if (rows.length === 0) return `<p class="irs-empty-row">Henüz sayısal ölçüm yok.</p>`;
  const hasLimits = char.lower_limit !== 0 || char.upper_limit !== 0;
  return `
    <table class="irs-char-table">
      <thead><tr><th>#</th><th>Ölçüm (Actual)</th>${hasLimits ? '<th>Durum</th>' : ''}<th>Tarih</th><th></th></tr></thead>
      <tbody>
        ${rows.map((r, i) => {
          const ok = !hasLimits ? null : r.actual >= char.lower_limit && r.actual <= char.upper_limit;
          const cls = ok === null ? '' : ok ? 'result-ok-row' : 'result-fail-row';
          const badge = ok === null ? '' : ok
            ? `<span class="result-badge result-ok">✓ İçi</span>`
            : `<span class="result-badge result-fail">✗ Dışı</span>`;
          return `<tr class="${cls}">
            <td>${i + 1}</td>
            <td class="meas-value">${r.actual}</td>
            ${hasLimits ? `<td>${badge}</td>` : ''}
            <td class="meas-date">${fmtDate(r.created_at)}</td>
            <td><button class="btn-icon-danger" data-del-numeric="${r.id}" title="Sil">🗑</button></td>
          </tr>`;
        }).join('')}
      </tbody>
    </table>`;
}

function renderCategoricalTable(rows) {
  if (rows.length === 0) return `<p class="irs-empty-row">Henüz kategorik ölçüm yok.</p>`;
  return `
    <table class="irs-char-table">
      <thead><tr><th>#</th><th>Index</th><th>Onaylı</th><th>Ek Bilgi</th><th>Tarih</th><th></th></tr></thead>
      <tbody>
        ${rows.map((r, i) => `
          <tr>
            <td>${i + 1}</td>
            <td>${r.index || '—'}</td>
            <td>${r.is_confirmed ? '<span class="result-badge result-ok">✓ Evet</span>' : '<span class="result-badge result-fail">✗ Hayır</span>'}</td>
            <td>${r.additional_info || '—'}</td>
            <td class="meas-date">${fmtDate(r.created_at)}</td>
            <td><button class="btn-icon-danger" data-del-cat="${r.id}" title="Sil">🗑</button></td>
          </tr>`).join('')}
      </tbody>
    </table>`;
}

function renderZoneTable(rows) {
  if (rows.length === 0) return `<p class="irs-empty-row">Henüz bölge ölçümü yok.</p>`;
  return `
    <table class="irs-char-table">
      <thead><tr><th>#</th><th>Bölge</th><th>Onaylı</th><th>Ek Bilgi</th><th>Tarih</th><th></th></tr></thead>
      <tbody>
        ${rows.map((r, i) => `
          <tr>
            <td>${i + 1}</td>
            <td>${r.zone_name || '—'}</td>
            <td>${r.is_confirmed ? '<span class="result-badge result-ok">✓ Evet</span>' : '<span class="result-badge result-fail">✗ Hayır</span>'}</td>
            <td>${r.additional_info || '—'}</td>
            <td class="meas-date">${fmtDate(r.created_at)}</td>
            <td><button class="btn-icon-danger" data-del-zone="${r.id}" title="Sil">🗑</button></td>
          </tr>`).join('')}
      </tbody>
    </table>`;
}

function openMeasurementModal(char, type, onSaved) {
  const isNumeric = type === 'numeric';
  const isCat    = type === 'categorical';
  const isZone   = type === 'zone';

  modal.open(`
    <div class="modal-header">
      <h3 class="modal-title">Ölçüm Ekle — Item ${char.item_no}</h3>
    </div>
    <div class="modal-body">
      <div class="meas-type-seg" id="meas-seg">
        <button class="seg-btn ${isNumeric ? 'active' : ''}" data-type="numeric">Sayısal</button>
        <button class="seg-btn ${isCat ? 'active' : ''}" data-type="categorical">Kategorik</button>
        <button class="seg-btn ${isZone ? 'active' : ''}" data-type="zone">Bölge</button>
      </div>

      <div id="mf-numeric" class="${isNumeric ? '' : 'hidden'}">
        <div class="form-group">
          <label>Ölçüm Değeri *</label>
          <input type="number" step="0.0001" id="mf-actual" class="form-input form-input-lg"
                 placeholder="${char.lower_limit && char.upper_limit
                   ? `Beklenen: ${char.lower_limit} – ${char.upper_limit}`
                   : 'Ölçüm değeri girin'}" />
          ${(char.lower_limit !== 0 || char.upper_limit !== 0) ? `
          <div class="form-hint">
            Alt: ${char.lower_limit.toFixed(4)} · Üst: ${char.upper_limit.toFixed(4)}
          </div>` : ''}
        </div>
      </div>

      <div id="mf-categorical" class="${isCat ? '' : 'hidden'}">
        <div class="form-row-2">
          <div class="form-group">
            <label>Index</label>
            <input type="text" id="mf-index" class="form-input" placeholder="ör. P1" />
          </div>
          <div class="form-group">
            <label>Onaylı?</label>
            <select id="mf-confirmed" class="form-input">
              <option value="true">Evet ✓</option>
              <option value="false">Hayır ✗</option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <label>Ek Bilgi</label>
          <input type="text" id="mf-catinfo" class="form-input" placeholder="Opsiyonel not" />
        </div>
      </div>

      <div id="mf-zone" class="${isZone ? '' : 'hidden'}">
        <div class="form-row-2">
          <div class="form-group">
            <label>Bölge Adı</label>
            <input type="text" id="mf-zone-name" class="form-input" placeholder="ör. ZONE-A" />
          </div>
          <div class="form-group">
            <label>Onaylı?</label>
            <select id="mf-zone-confirmed" class="form-input">
              <option value="true">Evet ✓</option>
              <option value="false">Hayır ✗</option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <label>Ek Bilgi</label>
          <input type="text" id="mf-zoneinfo" class="form-input" placeholder="Opsiyonel not" />
        </div>
      </div>

      <div id="mf-preview" class="meas-preview hidden"></div>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" id="mf-cancel">İptal</button>
      <button class="btn btn-primary" id="mf-save">Kaydet</button>
    </div>`);

  // Segmented control
  let activeType = type;
  document.querySelectorAll('.seg-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.seg-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      activeType = btn.dataset.type;
      ['numeric','categorical','zone'].forEach(t => {
        document.getElementById(`mf-${t}`)?.classList.toggle('hidden', t !== activeType);
      });
    });
  });

  // Live preview for numeric
  const actualInput = document.getElementById('mf-actual');
  const preview = document.getElementById('mf-preview');
  if (actualInput && (char.lower_limit !== 0 || char.upper_limit !== 0)) {
    actualInput.addEventListener('input', () => {
      const v = parseFloat(actualInput.value);
      if (isNaN(v)) { preview.classList.add('hidden'); return; }
      const ok = v >= char.lower_limit && v <= char.upper_limit;
      preview.className = `meas-preview ${ok ? 'preview-ok' : 'preview-fail'}`;
      preview.textContent = ok ? `✓ Tolerans İçi (${char.lower_limit} – ${char.upper_limit})` : `✗ Tolerans Dışı`;
    });
  }

  document.getElementById('mf-cancel').addEventListener('click', () => modal.close());
  document.getElementById('mf-save').addEventListener('click', async () => {
    try {
      if (activeType === 'numeric') {
        const v = parseFloat(document.getElementById('mf-actual').value);
        if (isNaN(v)) { toast('Geçerli bir sayı girin.', 'error'); return; }
        await api.numericResults.create({ actual: v, character_id: char.id });
      } else if (activeType === 'categorical') {
        await api.categoricalResults.create({
          index:           document.getElementById('mf-index').value.trim() || null,
          is_confirmed:    document.getElementById('mf-confirmed').value === 'true',
          additional_info: document.getElementById('mf-catinfo').value.trim() || null,
          character_id:    char.id,
        });
      } else {
        await api.zoneResults.create({
          zone_name:       document.getElementById('mf-zone-name').value.trim() || null,
          is_confirmed:    document.getElementById('mf-zone-confirmed').value === 'true',
          additional_info: document.getElementById('mf-zoneinfo').value.trim() || null,
          character_id:    char.id,
        });
      }
      modal.close();
      toast('Ölçüm kaydedildi.', 'success');
      onSaved?.();
    } catch (err) { toast(err.message, 'error'); }
  });
}

function infoChip(label, value) {
  return `<div class="info-chip"><span class="info-chip-label">${label}</span><span class="info-chip-value">${value}</span></div>`;
}

function fmtDate(iso) {
  if (!iso) return '—';
  return new Date(iso).toLocaleString('tr-TR', { day:'2-digit', month:'2-digit', year:'numeric', hour:'2-digit', minute:'2-digit' });
}

function getResultMeta(r) {
  const map = {
    WithinTolerance:           { label: 'Tolerans İçi',  cls: 'result-ok'    },
    OutOfTolerance:            { label: 'Tolerans Dışı', cls: 'result-fail'  },
    WrongFormat:               { label: 'Yanlış Format', cls: 'result-warn'  },
    MinMaxValueOverTolerance:  { label: 'Min/Max Fazla', cls: 'result-over'  },
    MinMaxValueUnderTolerance: { label: 'Min/Max Az',    cls: 'result-under' },
    MaxValueOverTolerance:     { label: 'Max Fazla',     cls: 'result-over'  },
    MinValueUnderTolerance:    { label: 'Min Az',        cls: 'result-under' },
    Unidentified:              { label: 'Belirsiz',      cls: 'result-none'  },
  };
  return map[r] || map.Unidentified;
}

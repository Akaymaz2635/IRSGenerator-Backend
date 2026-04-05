import { api }     from '../api.js';
import { session } from '../session.js';

// ── NCM Detail — NC selection + disposition sheet generation ──────────────
export async function ncmDetailPage(id) {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Yükleniyor...</div>';

  const canWrite = session.getRole() === 'engineer' || session.getRole() === 'admin';

  let data, causeCodes, dispTypes;
  try {
    [data, causeCodes, dispTypes] = await Promise.all([
      api.inspections.ncmData(id),
      api.causeCodes.list(true),
      api.ncmDispositionTypes.list(true),
    ]);
  } catch (err) {
    root.innerHTML = `<div class="empty text-danger">Hata: ${err.message}</div>`;
    return;
  }

  const totalNc = data.dimensional.length + data.visual.length;

  // Track selected NC items: key = "dim-{charId}" or "vis-{defectId}"
  const selected = new Set();

  function render() {
    const dimRows = data.dimensional.length === 0
      ? `<tr><td colspan="3" class="text-secondary" style="padding:12px;">Dimensional NC yok.</td></tr>`
      : data.dimensional.map((item) => {
          const key = `dim-${item.character_id}`;
          const checked = selected.has(key) ? 'checked' : '';
          const rowCls  = selected.has(key) ? 'ncm-selected-row' : '';
          return `
            <tr class="ncm-nc-row ${rowCls}" data-key="${key}">
              <td style="width:36px;">
                ${canWrite ? `<input type="checkbox" class="ncm-chk" data-key="${key}" ${checked}>` : ''}
              </td>
              <td style="width:80px;color:var(--text-secondary);font-size:12px;">${escHtml(item.item_no)}</td>
              <td class="nc-desc-text">${escHtml(item.description)}</td>
            </tr>`;
        }).join('')
      ;

    const visRows = data.visual.length === 0
      ? `<tr><td colspan="3" class="text-secondary" style="padding:12px;">Visual NC yok.</td></tr>`
      : data.visual.map((item) => {
          const key = `vis-${item.defect_id}`;
          const checked = selected.has(key) ? 'checked' : '';
          const rowCls  = selected.has(key) ? 'ncm-selected-row' : '';
          return `
            <tr class="ncm-nc-row ${rowCls}" data-key="${key}">
              <td style="width:36px;">
                ${canWrite ? `<input type="checkbox" class="ncm-chk" data-key="${key}" ${checked}>` : ''}
              </td>
              <td style="width:80px;color:var(--text-secondary);font-size:12px;">#${item.index}</td>
              <td class="nc-desc-text">${escHtml(item.description)}</td>
            </tr>`;
        }).join('')
      ;

    const causeOptions = causeCodes.map((c) =>
      `<option value="${c.id}">${escHtml(c.code)} — ${escHtml(c.description)}</option>`
    ).join('');

    const dispOptions = dispTypes.map((t) =>
      `<option value="${t.id}">${escHtml(t.label)} (${escHtml(t.code)})</option>`
    ).join('');

    const selCount = selected.size;

    root.innerHTML = `
      <a href="#/ncm" class="back-link">← NCM Listesi</a>

      <div class="page-header" style="margin-bottom:12px;">
        <h1>Nonconformance Management</h1>
      </div>

      <!-- Inspection info card -->
      <div class="card" style="margin-bottom:16px;">
        <div class="card-header"><span class="card-title">Inspection #${id}</span></div>
        <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(160px,1fr));gap:8px 24px;padding:12px 16px 16px;font-size:13px;">
          <div><span class="text-secondary">Part No</span><br><strong>${escHtml(data.part_number || '—')}</strong></div>
          <div><span class="text-secondary">Serial No</span><br><strong>${escHtml(data.serial_number || '—')}</strong></div>
          <div><span class="text-secondary">Operation</span><br><strong>${escHtml(data.operation_number || '—')}</strong></div>
          <div><span class="text-secondary">Inspector</span><br><strong>${escHtml(data.inspector || '—')}</strong></div>
          <div><span class="text-secondary">Project</span><br><strong>${escHtml(data.project_name || '—')}</strong></div>
          <div><span class="text-secondary">Total NC</span><br><strong>${totalNc}</strong></div>
        </div>
      </div>

      <div style="display:grid;grid-template-columns:1fr 360px;gap:16px;align-items:start;">

        <!-- Left: NC tables -->
        <div>
          <!-- Dimensional -->
          <div class="card" style="margin-bottom:16px;">
            <div class="card-header">
              <span class="card-title">Dimensional NC (${data.dimensional.length})</span>
              ${canWrite && data.dimensional.length > 0 ? `
                <label style="font-size:13px;display:flex;align-items:center;gap:6px;cursor:pointer;">
                  <input type="checkbox" id="chk-all-dim"> Tümünü seç
                </label>` : ''}
            </div>
            <table class="data-table nc-desc-table">
              <tbody id="dim-tbody">${dimRows}</tbody>
            </table>
          </div>

          <!-- Visual -->
          <div class="card">
            <div class="card-header">
              <span class="card-title">Visual NC (${data.visual.length})</span>
              ${canWrite && data.visual.length > 0 ? `
                <label style="font-size:13px;display:flex;align-items:center;gap:6px;cursor:pointer;">
                  <input type="checkbox" id="chk-all-vis"> Tümünü seç
                </label>` : ''}
            </div>
            <table class="data-table nc-desc-table">
              <tbody id="vis-tbody">${visRows}</tbody>
            </table>
          </div>
        </div>

        <!-- Right: Disposition form -->
        ${canWrite ? `
        <div class="card" style="position:sticky;top:80px;">
          <div class="card-header">
            <span class="card-title">Disposition Sheet</span>
            <span class="badge badge-info" id="sel-count-badge">${selCount} NC seçili</span>
          </div>
          <div style="padding:16px;display:flex;flex-direction:column;gap:12px;">

            <div class="form-group">
              <label class="form-label">OPER <span class="text-danger">*</span></label>
              <input id="ncm-oper" type="number" class="form-control" placeholder="ör. 100" min="0">
            </div>

            <div class="form-group">
              <label class="form-label">C-OP (Cause Operation) <span class="text-danger">*</span></label>
              <input id="ncm-cop" type="number" class="form-control" placeholder="ör. 90" min="0">
            </div>

            <div class="form-group">
              <label class="form-label">QTY</label>
              <input id="ncm-qty" type="number" class="form-control" value="1" min="1">
            </div>

            <div class="form-group">
              <label class="form-label">Cause Code <span class="text-danger">*</span></label>
              <select id="ncm-cause-code" class="form-control">
                <option value="">— Seçiniz —</option>
                ${causeOptions}
              </select>
            </div>

            <div class="form-group">
              <label class="form-label">Disposition Type <span class="text-danger">*</span></label>
              <select id="ncm-disp-type" class="form-control">
                <option value="">— Seçiniz —</option>
                ${dispOptions}
              </select>
            </div>

            <button id="ncm-generate-btn" class="btn btn-primary"
                    ${selCount === 0 ? 'disabled' : ''}>
              Disposition Sheet Oluştur (${selCount} NC)
            </button>

            <div id="ncm-result" style="display:none;"></div>
          </div>
        </div>` : `
        <div class="card">
          <div style="padding:24px;text-align:center;color:var(--text-secondary);">
            <div style="font-size:32px;margin-bottom:8px;">👁</div>
            <div>Inspector — sadece okuma yetkisi.</div>
          </div>
        </div>`}

      </div>
    `;

    attachEvents();
  }

  function attachEvents() {
    // Individual checkboxes
    root.querySelectorAll('.ncm-chk').forEach((chk) => {
      chk.addEventListener('change', () => {
        if (chk.checked) selected.add(chk.dataset.key);
        else             selected.delete(chk.dataset.key);
        updateForm();
      });
    });

    // Row click toggles checkbox
    root.querySelectorAll('.ncm-nc-row').forEach((tr) => {
      tr.addEventListener('click', (e) => {
        if (e.target.type === 'checkbox') return;
        const chk = tr.querySelector('.ncm-chk');
        if (!chk) return;
        chk.checked = !chk.checked;
        if (chk.checked) selected.add(chk.dataset.key);
        else             selected.delete(chk.dataset.key);
        updateForm();
      });
    });

    // Select all — dimensional
    const chkAllDim = root.querySelector('#chk-all-dim');
    if (chkAllDim) {
      chkAllDim.addEventListener('change', () => {
        data.dimensional.forEach((item) => {
          const key = `dim-${item.character_id}`;
          if (chkAllDim.checked) selected.add(key);
          else                   selected.delete(key);
        });
        updateForm();
      });
    }

    // Select all — visual
    const chkAllVis = root.querySelector('#chk-all-vis');
    if (chkAllVis) {
      chkAllVis.addEventListener('change', () => {
        data.visual.forEach((item) => {
          const key = `vis-${item.defect_id}`;
          if (chkAllVis.checked) selected.add(key);
          else                   selected.delete(key);
        });
        updateForm();
      });
    }

    // Generate button
    const genBtn = root.querySelector('#ncm-generate-btn');
    if (genBtn) {
      genBtn.addEventListener('click', handleGenerate);
    }
  }

  function updateForm() {
    const selCount = selected.size;

    // Update row highlight
    root.querySelectorAll('.ncm-nc-row').forEach((tr) => {
      tr.classList.toggle('ncm-selected-row', selected.has(tr.dataset.key));
    });
    root.querySelectorAll('.ncm-chk').forEach((chk) => {
      chk.checked = selected.has(chk.dataset.key);
    });

    // Update badge + button
    const badge = root.querySelector('#sel-count-badge');
    if (badge) badge.textContent = `${selCount} NC seçili`;

    const genBtn = root.querySelector('#ncm-generate-btn');
    if (genBtn) {
      genBtn.disabled    = selCount === 0;
      genBtn.textContent = `Disposition Sheet Oluştur (${selCount} NC)`;
    }
  }

  async function handleGenerate() {
    const oper       = root.querySelector('#ncm-oper')?.value?.trim();
    const cop        = root.querySelector('#ncm-cop')?.value?.trim();
    const qty        = parseInt(root.querySelector('#ncm-qty')?.value || '1', 10);
    const causeCodeId  = parseInt(root.querySelector('#ncm-cause-code')?.value || '0', 10);
    const dispTypeId   = parseInt(root.querySelector('#ncm-disp-type')?.value  || '0', 10);

    if (!oper)        { toast('OPER zorunludur.', 'error'); return; }
    if (!cop)         { toast('C-OP zorunludur.', 'error'); return; }
    if (!causeCodeId) { toast('Cause Code seçiniz.', 'error'); return; }
    if (!dispTypeId)  { toast('Disposition Type seçiniz.', 'error'); return; }
    if (selected.size === 0) { toast('En az bir NC seçiniz.', 'error'); return; }

    // Build ordered items list (dimensional first, then visual, preserving order)
    const items = [];
    data.dimensional.forEach((item) => {
      if (selected.has(`dim-${item.character_id}`))
        items.push({ type: 'dimensional', source_id: item.character_id, description: item.description });
    });
    data.visual.forEach((item) => {
      if (selected.has(`vis-${item.defect_id}`))
        items.push({ type: 'visual', source_id: item.defect_id, description: item.description });
    });

    const genBtn = root.querySelector('#ncm-generate-btn');
    genBtn.disabled = true;
    genBtn.textContent = 'Oluşturuluyor...';

    try {
      const { blob, fileName } = await api.ncm.generate({
        inspection_id:       id,
        oper,
        cause_oper:          cop,
        qty,
        cause_code_id:       causeCodeId,
        disposition_type_id: dispTypeId,
        items,
      });

      // Trigger browser download
      const url = URL.createObjectURL(blob);
      const a   = document.createElement('a');
      a.href     = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);

      const resultDiv = root.querySelector('#ncm-result');
      resultDiv.style.display = 'block';
      resultDiv.innerHTML = `
        <div class="alert alert-success" style="margin-top:8px;">
          <strong>Disposition sheet indirildi:</strong> ${escHtml(fileName)}
        </div>`;

      toast('Disposition sheet indirildi.', 'success');

      // Mark selected rows as "done"
      selected.forEach((key) => {
        const tr = root.querySelector(`.ncm-nc-row[data-key="${key}"]`);
        if (tr) tr.classList.add('ncm-done-row');
      });
      selected.clear();
      updateForm();
    } catch (err) {
      toast(`Hata: ${err.message}`, 'error');
    } finally {
      genBtn.disabled    = false;
      genBtn.textContent = `Disposition Sheet Oluştur (${selected.size} NC)`;
    }
  }

  render();
}

// ── Helpers ───────────────────────────────────────────────────────────────
function escHtml(str) {
  return String(str ?? '')
    .replace(/&/g, '&amp;').replace(/</g, '&lt;')
    .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

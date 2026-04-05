import { api } from '../api.js';
import { session } from '../session.js';

export async function inspectionFormPage(id) {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Yükleniyor...</div>';

  if (!session.canWrite()) {
    root.innerHTML = '<div class="empty text-danger">Bu işlem için yetkiniz bulunmuyor.</div>';
    return;
  }

  const isEdit = id !== null;
  const [projects, inspection] = await Promise.all([
    api.projects.list(),
    isEdit ? api.inspections.get(id) : Promise.resolve(null),
  ]);

  if (isEdit && !inspection) {
    root.innerHTML = '<div class="empty text-danger">Inspection not found.</div>';
    return;
  }

  const v = inspection || {};
  const title = isEdit ? `Edit Inspection #${id}` : 'New Inspection';

  const projectOptions = projects.map((p) =>
    `<option value="${p.id}" ${v.project_id == p.id ? 'selected' : ''}>${p.name}</option>`
  ).join('');

  root.innerHTML = `
    <a href="${isEdit ? `#/inspections/${id}` : '#/inspections'}" class="back-link">&#8592; Geri</a>

    <div class="page-header">
      <h1>${title}</h1>
    </div>

    <div class="card">
      <form id="insp-form" novalidate>
        <div class="form-grid">
          <div class="form-group">
            <label for="project_id">Engine Project</label>
            <select class="form-select" id="project_id" name="project_id">
              <option value="">— Select —</option>
              ${projectOptions}
            </select>
          </div>

          <div class="form-group">
            <label for="part_number">Part Number</label>
            <input type="text" class="form-input" id="part_number" name="part_number"
              value="${v.part_number || ''}" placeholder="e.g. PN-12345" />
          </div>

          <div class="form-group">
            <label for="serial_number">Serial Number</label>
            <input type="text" class="form-input" id="serial_number" name="serial_number"
              value="${v.serial_number || ''}" placeholder="e.g. SN-00987" />
          </div>

          <div class="form-group">
            <label for="operation_number">Operation Number</label>
            <input type="text" class="form-input" id="operation_number" name="operation_number"
              value="${v.operation_number || ''}" placeholder="e.g. OP-010" />
          </div>

          <div class="form-group">
            <label for="inspector">Inspector</label>
            <input type="text" class="form-input" id="inspector" name="inspector"
              value="${v.inspector || (!isEdit ? session.getName() : '')}" placeholder="Full name" />
          </div>

          ${!isEdit ? `
          <div class="form-group">
            <label for="op_sheet_file">Op Sheet (.docx) — If Dimensional Measurement Will Be Done</label>
            <input type="file" class="form-input" id="op_sheet_file" accept=".docx" />
            <small class="form-hint">When uploaded, dimensional characteristics are parsed automatically.</small>
          </div>` : ''}

          <div class="form-group-full form-group">
            <label for="notes">Notes</label>
            <textarea class="form-textarea" id="notes" name="notes" rows="4"
              placeholder="Inspection notes...">${v.notes || ''}</textarea>
          </div>
        </div>

        <div class="form-actions">
          <a href="${isEdit ? `#/inspections/${id}` : '#/inspections'}" class="btn btn-ghost">İptal</a>
          <button type="submit" class="btn btn-primary" id="submit-btn">
            ${isEdit ? 'Kaydet' : 'Oluştur'}
          </button>
        </div>
      </form>
    </div>
  `;

  const form = root.querySelector('#insp-form');
  const submitBtn = root.querySelector('#submit-btn');

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    submitBtn.disabled = true;
    submitBtn.textContent = 'Kaydediliyor...';

    const fd = new FormData(form);
    const data = {
      project_id:       fd.get('project_id')       ? Number(fd.get('project_id')) : null,
      part_number:      fd.get('part_number')       || null,
      serial_number:    fd.get('serial_number')     || null,
      operation_number: fd.get('operation_number')  || null,
      inspector:        fd.get('inspector')          || null,
      status:           'open',
      notes:            fd.get('notes')              || null,
    };

    try {
      if (isEdit) {
        await api.inspections.update(id, data);
        window.toast('Inspection updated.', 'success');
        window.navigate(`/inspections/${id}`);
      } else {
        const created = await api.inspections.create(data);

        // Upload op sheet if provided
        const opSheetInput = document.getElementById('op_sheet_file');
        if (opSheetInput && opSheetInput.files.length > 0) {
          try {
            submitBtn.textContent = 'Op Sheet parse ediliyor...';
            const result = await api.inspections.parseOpSheet(created.id, opSheetInput.files[0]);
            window.toast(`Inspection created. ${result.characters_created} characteristics parsed.`, 'success');
          } catch (parseErr) {
            window.toast(`Muayene oluşturuldu, ancak op sheet parse hatası: ${parseErr.message}`, 'warning');
          }
        } else {
          window.toast('Inspection created.', 'success');
        }

        window.navigate(`/inspections/${created.id}`);
      }
    } catch (err) {
      window.toast('Hata: ' + err.message, 'error');
      submitBtn.disabled = false;
      submitBtn.textContent = isEdit ? 'Kaydet' : 'Oluştur';
    }
  });
}

import { api } from '../api.js';
import { session } from '../session.js';

export async function settingsPage() {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Yükleniyor...</div>';

  if (!session.isAdmin()) {
    root.innerHTML = '<div class="admin-empty">Bu sayfaya erişim yetkiniz yok.</div>';
    return;
  }

  let activeTab = 'projects';

  async function render() {
    root.innerHTML = `
      <div class="page-header">
        <h1>Ayarlar</h1>
      </div>

      <div class="tabs">
        <button class="tab-btn ${activeTab === 'projects' ? 'active' : ''}" data-tab="projects">
          Motor Projeleri
        </button>
        <button class="tab-btn ${activeTab === 'defect-types' ? 'active' : ''}" data-tab="defect-types">
          Hata Tipleri
        </button>
        <button class="tab-btn ${activeTab === 'cause-codes' ? 'active' : ''}" data-tab="cause-codes">
          Cause Codes
        </button>
        <button class="tab-btn ${activeTab === 'ncm-disp-types' ? 'active' : ''}" data-tab="ncm-disp-types">
          NCM Disposition Types
        </button>
      </div>

      <div id="tab-content">
        <div class="loading">Yükleniyor...</div>
      </div>
    `;

    root.querySelectorAll('.tab-btn').forEach((btn) => {
      btn.addEventListener('click', async () => {
        activeTab = btn.dataset.tab;
        await render();
      });
    });

    const tabContent = root.querySelector('#tab-content');
    if (activeTab === 'projects')          await renderProjectsTab(tabContent);
    else if (activeTab === 'defect-types') await renderDefectTypesTab(tabContent);
    else if (activeTab === 'cause-codes')  await renderCauseCodesTab(tabContent);
    else if (activeTab === 'ncm-disp-types') await renderNcmDispTypesTab(tabContent);
  }

  await render();
}

// ── Projects Tab ─────────────────────────────────────────────
async function renderProjectsTab(container) {
  const projects = await api.projects.list();

  const rows = projects.map((p) => `
    <tr>
      <td class="col-id">#${p.id}</td>
      <td>${p.name}</td>
      <td>${p.customer || '—'}</td>
      <td class="text-secondary">${p.description || '—'}</td>
      <td class="col-actions">
        <button class="btn btn-ghost btn-xs edit-project-btn" data-id="${p.id}">Düzenle</button>
        <button class="btn btn-danger btn-xs delete-project-btn" data-id="${p.id}" data-name="${p.name}">Pasife Al</button>
      </td>
    </tr>
  `).join('');

  container.innerHTML = `
    <div class="card">
      <div class="card-header">
        <span class="card-title">Projeler (${projects.length})</span>
        <button class="btn btn-primary btn-sm" id="add-project-btn">+ Proje Ekle</button>
      </div>
      <div class="table-wrapper">
        ${
          projects.length === 0
            ? '<div class="empty">Proje bulunamadı.</div>'
            : `<table class="data-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Ad</th>
                    <th>Müşteri</th>
                    <th>Açıklama</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>${rows}</tbody>
              </table>`
        }
      </div>
    </div>
  `;

  container.querySelector('#add-project-btn').addEventListener('click', () => {
    openProjectModal(null, () => renderProjectsTab(container));
  });

  container.querySelectorAll('.edit-project-btn').forEach((btn) => {
    btn.addEventListener('click', async () => {
      const id = Number(btn.dataset.id);
      const proj = projects.find((p) => p.id === id);
      openProjectModal(proj, () => renderProjectsTab(container));
    });
  });

  container.querySelectorAll('.delete-project-btn').forEach((btn) => {
    btn.addEventListener('click', async () => {
      const id = Number(btn.dataset.id);
      const name = btn.dataset.name || `#${id}`;
      if (!confirm(`"${name}" pasife alınacak. Onaylıyor musunuz?`)) return;
      try {
        await api.projects.delete(id);
        window.toast('Proje pasife alındı.', 'success');
        await renderProjectsTab(container);
      } catch (err) {
        window.toast('Hata: ' + err.message, 'error');
      }
    });
  });
}

function openProjectModal(existing, onDone) {
  const isEdit = existing !== null && existing !== undefined;
  const v = existing || {};
  const title = isEdit ? `Proje Düzenle #${v.id}` : 'Yeni Proje';

  const html = `
    <div class="modal-card">
      <div class="modal-header">
        <span class="modal-title">${title}</span>
        <button class="modal-close" id="modal-close-btn">&times;</button>
      </div>
      <form id="project-form" novalidate>
        <div class="form-grid">
          <div class="form-group form-group-full">
            <label>Proje Adı *</label>
            <input type="text" class="form-input" name="name"
              value="${v.name || ''}" placeholder="Proje adı..." required />
          </div>
          <div class="form-group">
            <label>Müşteri</label>
            <input type="text" class="form-input" name="customer"
              value="${v.customer || ''}" placeholder="Müşteri adı..." />
          </div>
          <div class="form-group form-group-full">
            <label>Açıklama</label>
            <textarea class="form-textarea" name="description" rows="3"
              placeholder="Proje açıklaması...">${v.description || ''}</textarea>
          </div>
        </div>
        <div class="form-actions">
          <button type="button" class="btn btn-ghost" id="modal-cancel-btn">İptal</button>
          <button type="submit" class="btn btn-primary" id="proj-submit-btn">
            ${isEdit ? 'Kaydet' : 'Oluştur'}
          </button>
        </div>
      </form>
    </div>
  `;

  window.modal.open(html);
  document.getElementById('modal-close-btn').addEventListener('click', () => window.modal.close());
  document.getElementById('modal-cancel-btn').addEventListener('click', () => window.modal.close());

  document.getElementById('project-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const submitBtn = document.getElementById('proj-submit-btn');
    submitBtn.disabled = true;
    const fd = new FormData(e.target);
    const data = {
      name:        fd.get('name') || '',
      customer:    fd.get('customer') || null,
      description: fd.get('description') || null,
    };
    if (!data.name.trim()) {
      window.toast('Proje adı zorunludur.', 'error');
      submitBtn.disabled = false;
      return;
    }
    try {
      if (isEdit) {
        await api.projects.update(v.id, data);
        window.toast('Proje güncellendi.', 'success');
      } else {
        await api.projects.create(data);
        window.toast('Proje oluşturuldu.', 'success');
      }
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Hata: ' + err.message, 'error');
      submitBtn.disabled = false;
    }
  });
}

// ── Defect Types Tab ──────────────────────────────────────────
async function renderDefectTypesTab(container) {
  const defectTypes = await api.defectTypes.list();

  const rows = defectTypes.map((dt) => `
    <tr>
      <td class="col-id">#${dt.id}</td>
      <td>${dt.name}</td>
      <td class="text-secondary">${dt.description || '—'}</td>
      <td class="col-actions">
        <button class="btn btn-ghost btn-xs edit-dt-btn" data-id="${dt.id}">Düzenle</button>
        <button class="btn btn-danger btn-xs delete-dt-btn" data-id="${dt.id}" data-name="${dt.name}">Pasife Al</button>
      </td>
    </tr>
  `).join('');

  container.innerHTML = `
    <div class="card">
      <div class="card-header">
        <span class="card-title">Hata Tipleri (${defectTypes.length})</span>
        <button class="btn btn-primary btn-sm" id="add-dt-btn">+ Hata Tipi Ekle</button>
      </div>
      <div class="table-wrapper">
        ${
          defectTypes.length === 0
            ? '<div class="empty">Hata tipi bulunamadı.</div>'
            : `<table class="data-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Ad</th>
                    <th>Açıklama</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>${rows}</tbody>
              </table>`
        }
      </div>
    </div>
  `;

  container.querySelector('#add-dt-btn').addEventListener('click', () => {
    openDefectTypeModal(null, () => renderDefectTypesTab(container));
  });

  container.querySelectorAll('.edit-dt-btn').forEach((btn) => {
    btn.addEventListener('click', async () => {
      const id = Number(btn.dataset.id);
      const dt = defectTypes.find((d) => d.id === id);
      openDefectTypeModal(dt, () => renderDefectTypesTab(container));
    });
  });

  container.querySelectorAll('.delete-dt-btn').forEach((btn) => {
    btn.addEventListener('click', async () => {
      const id = Number(btn.dataset.id);
      const name = btn.dataset.name || `#${id}`;
      if (!confirm(`"${name}" pasife alınacak. Onaylıyor musunuz?`)) return;
      try {
        await api.defectTypes.delete(id);
        window.toast('Hata tipi pasife alındı.', 'success');
        await renderDefectTypesTab(container);
      } catch (err) {
        window.toast('Hata: ' + err.message, 'error');
      }
    });
  });
}

function openDefectTypeModal(existing, onDone) {
  const isEdit = existing !== null && existing !== undefined;
  const v = existing || {};
  const title = isEdit ? `Hata Tipi Düzenle #${v.id}` : 'Yeni Hata Tipi';

  const html = `
    <div class="modal-card">
      <div class="modal-header">
        <span class="modal-title">${title}</span>
        <button class="modal-close" id="modal-close-btn">&times;</button>
      </div>
      <form id="dt-form" novalidate>
        <div class="form-grid">
          <div class="form-group form-group-full">
            <label>Hata Tipi Adı *</label>
            <input type="text" class="form-input" name="name"
              value="${v.name || ''}" placeholder="Hata tipi adı..." required />
          </div>
          <div class="form-group form-group-full">
            <label>Açıklama</label>
            <textarea class="form-textarea" name="description" rows="3"
              placeholder="Hata tipi açıklaması...">${v.description || ''}</textarea>
          </div>
        </div>
        <div class="form-actions">
          <button type="button" class="btn btn-ghost" id="modal-cancel-btn">İptal</button>
          <button type="submit" class="btn btn-primary" id="dt-submit-btn">
            ${isEdit ? 'Kaydet' : 'Oluştur'}
          </button>
        </div>
      </form>
    </div>
  `;

  window.modal.open(html);
  document.getElementById('modal-close-btn').addEventListener('click', () => window.modal.close());
  document.getElementById('modal-cancel-btn').addEventListener('click', () => window.modal.close());

  document.getElementById('dt-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const submitBtn = document.getElementById('dt-submit-btn');
    submitBtn.disabled = true;
    const fd = new FormData(e.target);
    const data = {
      name:        fd.get('name') || '',
      description: fd.get('description') || null,
    };
    if (!data.name.trim()) {
      window.toast('Hata tipi adı zorunludur.', 'error');
      submitBtn.disabled = false;
      return;
    }
    try {
      if (isEdit) {
        await api.defectTypes.update(v.id, data);
        window.toast('Hata tipi güncellendi.', 'success');
      } else {
        await api.defectTypes.create(data);
        window.toast('Hata tipi oluşturuldu.', 'success');
      }
      window.modal.close();
      await onDone();
    } catch (err) {
      window.toast('Hata: ' + err.message, 'error');
      submitBtn.disabled = false;
    }
  });
}

// ── Cause Codes Tab ────────────────────────────────────────────────────────
async function renderCauseCodesTab(container) {
  const items = await api.causeCodes.list();

  const rows = items.map((c) => `
    <tr>
      <td class="col-id">#${c.id}</td>
      <td><strong>${escHtml(c.code)}</strong></td>
      <td>${escHtml(c.description)}</td>
      <td>${c.sort_order}</td>
      <td><span class="badge ${c.active ? 'badge-success' : 'badge-neutral'}">${c.active ? 'Aktif' : 'Pasif'}</span></td>
      <td class="col-actions">
        <button class="btn btn-ghost btn-xs edit-cc-btn" data-id="${c.id}">Düzenle</button>
        <button class="btn btn-danger btn-xs delete-cc-btn" data-id="${c.id}" data-code="${escHtml(c.code)}">Sil</button>
      </td>
    </tr>`).join('');

  container.innerHTML = `
    <div class="card">
      <div class="card-header">
        <span class="card-title">Cause Codes (${items.length})</span>
        <button class="btn btn-primary btn-sm" id="add-cc-btn">+ Ekle</button>
      </div>
      ${items.length === 0
        ? '<div class="empty">Kayıt yok.</div>'
        : `<table class="data-table">
            <thead><tr>
              <th class="col-id">#</th><th>Code</th><th>Description</th>
              <th>Sort</th><th>Durum</th><th>İşlem</th>
            </tr></thead>
            <tbody>${rows}</tbody>
          </table>`}
    </div>`;

  container.querySelector('#add-cc-btn')?.addEventListener('click', () =>
    openCauseCodeModal(null, () => renderCauseCodesTab(container)));

  container.querySelectorAll('.edit-cc-btn').forEach((btn) =>
    btn.addEventListener('click', async () => {
      const item = items.find((c) => c.id === Number(btn.dataset.id));
      if (item) openCauseCodeModal(item, () => renderCauseCodesTab(container));
    }));

  container.querySelectorAll('.delete-cc-btn').forEach((btn) =>
    btn.addEventListener('click', async () => {
      if (!confirm(`"${btn.dataset.code}" silinsin mi?`)) return;
      await api.causeCodes.delete(Number(btn.dataset.id));
      window.toast('Silindi.', 'success');
      await renderCauseCodesTab(container);
    }));
}

function openCauseCodeModal(existing, onDone) {
  const isEdit = !!existing;
  window.modal.open(`
    <h2 class="modal-title">${isEdit ? 'Cause Code Düzenle' : 'Yeni Cause Code'}</h2>
    <div class="form-group">
      <label class="form-label">Code <span class="text-danger">*</span></label>
      <input id="cc-code" class="form-control" value="${escHtml(existing?.code || '')}" placeholder="ör. CC001">
    </div>
    <div class="form-group">
      <label class="form-label">Description <span class="text-danger">*</span></label>
      <input id="cc-desc" class="form-control" value="${escHtml(existing?.description || '')}" placeholder="ör. Operator Error">
    </div>
    <div class="form-group">
      <label class="form-label">Sort Order</label>
      <input id="cc-sort" type="number" class="form-control" value="${existing?.sort_order ?? 0}">
    </div>
    <div class="form-group">
      <label style="display:flex;align-items:center;gap:8px;cursor:pointer;">
        <input id="cc-active" type="checkbox" ${(existing?.active ?? true) ? 'checked' : ''}>
        Aktif
      </label>
    </div>
    <div class="modal-actions">
      <button class="btn btn-ghost" id="cc-cancel">İptal</button>
      <button class="btn btn-primary" id="cc-save">Kaydet</button>
    </div>`);

  document.getElementById('cc-cancel').addEventListener('click', () => window.modal.close());
  document.getElementById('cc-save').addEventListener('click', async () => {
    const code = document.getElementById('cc-code').value.trim();
    const desc = document.getElementById('cc-desc').value.trim();
    if (!code || !desc) { window.toast('Code ve Description zorunludur.', 'error'); return; }
    const data = {
      code, description: desc,
      sort_order: parseInt(document.getElementById('cc-sort').value || '0', 10),
      active: document.getElementById('cc-active').checked,
    };
    try {
      if (isEdit) await api.causeCodes.update(existing.id, data);
      else        await api.causeCodes.create(data);
      window.toast(isEdit ? 'Güncellendi.' : 'Oluşturuldu.', 'success');
      window.modal.close();
      await onDone();
    } catch (err) { window.toast('Hata: ' + err.message, 'error'); }
  });
}

// ── NCM Disposition Types Tab ──────────────────────────────────────────────
async function renderNcmDispTypesTab(container) {
  const items = await api.ncmDispositionTypes.list();

  const rows = items.map((t) => `
    <tr>
      <td class="col-id">#${t.id}</td>
      <td><strong>${escHtml(t.code)}</strong></td>
      <td>${escHtml(t.label)}</td>
      <td class="text-secondary" style="font-size:12px;">${escHtml(t.description)}</td>
      <td style="font-size:12px;">${escHtml(t.template_file_name)}</td>
      <td>${t.sort_order}</td>
      <td><span class="badge ${t.active ? 'badge-success' : 'badge-neutral'}">${t.active ? 'Aktif' : 'Pasif'}</span></td>
      <td class="col-actions">
        <button class="btn btn-ghost btn-xs edit-ndt-btn" data-id="${t.id}">Düzenle</button>
        <button class="btn btn-danger btn-xs delete-ndt-btn" data-id="${t.id}" data-code="${escHtml(t.code)}">Sil</button>
      </td>
    </tr>`).join('');

  container.innerHTML = `
    <div class="card">
      <div class="card-header">
        <span class="card-title">NCM Disposition Types (${items.length})</span>
        <button class="btn btn-primary btn-sm" id="add-ndt-btn">+ Ekle</button>
      </div>
      ${items.length === 0
        ? '<div class="empty">Kayıt yok.</div>'
        : `<table class="data-table">
            <thead><tr>
              <th class="col-id">#</th><th>Code</th><th>Label</th><th>Description</th>
              <th>Template File</th><th>Sort</th><th>Durum</th><th>İşlem</th>
            </tr></thead>
            <tbody>${rows}</tbody>
          </table>`}
    </div>`;

  container.querySelector('#add-ndt-btn')?.addEventListener('click', () =>
    openNcmDispTypeModal(null, () => renderNcmDispTypesTab(container)));

  container.querySelectorAll('.edit-ndt-btn').forEach((btn) =>
    btn.addEventListener('click', () => {
      const item = items.find((t) => t.id === Number(btn.dataset.id));
      if (item) openNcmDispTypeModal(item, () => renderNcmDispTypesTab(container));
    }));

  container.querySelectorAll('.delete-ndt-btn').forEach((btn) =>
    btn.addEventListener('click', async () => {
      if (!confirm(`"${btn.dataset.code}" silinsin mi?`)) return;
      await api.ncmDispositionTypes.delete(Number(btn.dataset.id));
      window.toast('Silindi.', 'success');
      await renderNcmDispTypesTab(container);
    }));
}

function openNcmDispTypeModal(existing, onDone) {
  const isEdit = !!existing;
  window.modal.open(`
    <h2 class="modal-title">${isEdit ? 'NCM Disp. Type Düzenle' : 'Yeni NCM Disposition Type'}</h2>
    <div class="form-group">
      <label class="form-label">Code <span class="text-danger">*</span></label>
      <input id="ndt-code" class="form-control" value="${escHtml(existing?.code || '')}" placeholder="ör. ACCEPT">
    </div>
    <div class="form-group">
      <label class="form-label">Label <span class="text-danger">*</span></label>
      <input id="ndt-label" class="form-control" value="${escHtml(existing?.label || '')}" placeholder="ör. Accept (Use As Is)">
    </div>
    <div class="form-group">
      <label class="form-label">Description</label>
      <input id="ndt-desc" class="form-control" value="${escHtml(existing?.description || '')}" placeholder="Kısa açıklama">
    </div>
    <div class="form-group">
      <label class="form-label">Template File Name <span class="text-danger">*</span>
        <span class="text-secondary" style="font-size:11px;"> (DispositionTemplates klasöründeki .docx adı)</span>
      </label>
      <input id="ndt-tmpl" class="form-control" value="${escHtml(existing?.template_file_name || '')}" placeholder="ör. ACCEPT.docx">
    </div>
    <div class="form-group">
      <label class="form-label">Sort Order</label>
      <input id="ndt-sort" type="number" class="form-control" value="${existing?.sort_order ?? 0}">
    </div>
    <div class="form-group">
      <label style="display:flex;align-items:center;gap:8px;cursor:pointer;">
        <input id="ndt-active" type="checkbox" ${(existing?.active ?? true) ? 'checked' : ''}>
        Aktif
      </label>
    </div>
    <div class="modal-actions">
      <button class="btn btn-ghost" id="ndt-cancel">İptal</button>
      <button class="btn btn-primary" id="ndt-save">Kaydet</button>
    </div>`);

  document.getElementById('ndt-cancel').addEventListener('click', () => window.modal.close());
  document.getElementById('ndt-save').addEventListener('click', async () => {
    const code  = document.getElementById('ndt-code').value.trim();
    const label = document.getElementById('ndt-label').value.trim();
    const tmpl  = document.getElementById('ndt-tmpl').value.trim();
    if (!code || !label || !tmpl) {
      window.toast('Code, Label ve Template File zorunludur.', 'error'); return;
    }
    const data = {
      code, label, template_file_name: tmpl,
      description: document.getElementById('ndt-desc').value.trim(),
      sort_order: parseInt(document.getElementById('ndt-sort').value || '0', 10),
      active: document.getElementById('ndt-active').checked,
    };
    try {
      if (isEdit) await api.ncmDispositionTypes.update(existing.id, data);
      else        await api.ncmDispositionTypes.create(data);
      window.toast(isEdit ? 'Güncellendi.' : 'Oluşturuldu.', 'success');
      window.modal.close();
      await onDone();
    } catch (err) { window.toast('Hata: ' + err.message, 'error'); }
  });
}

function escHtml(str) {
  return String(str ?? '')
    .replace(/&/g, '&amp;').replace(/</g, '&lt;')
    .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

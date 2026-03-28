import { api } from '../api.js';

export async function adminProjectsPage(container) {
  await renderList(container);
}

async function renderList(container) {
  let projects;
  try {
    projects = await api.projects.list(true);
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Projeler</h1>
        <p>${projects.length} kayıt</p>
      </div>
      <button class="btn btn-primary" id="btn-add-project">+ Yeni Proje</button>
    </div>

    <div class="admin-card">
      <div class="admin-table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Proje Adı</th>
              <th>Durum</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            ${projects.length === 0 ? '<tr><td colspan="4" style="text-align:center;color:var(--text-muted);padding:32px;">Proje bulunamadı</td></tr>' :
              projects.map(p => `
                <tr>
                  <td style="color:var(--text-muted);font-size:12px">#${p.id}</td>
                  <td>${esc(p.name)}</td>
                  <td>
                    <span class="badge ${p.active ? 'badge-active' : 'badge-inactive'}">
                      ${p.active ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td>
                    <div class="tbl-actions">
                      <button class="btn btn-sm btn-ghost btn-edit" data-id="${p.id}">Düzenle</button>
                      ${p.active
                        ? `<button class="btn btn-sm btn-danger btn-toggle" data-id="${p.id}" data-name="${esc(p.name)}" data-active="1">Pasife Al</button>`
                        : `<button class="btn btn-sm btn-secondary btn-toggle" data-id="${p.id}" data-name="${esc(p.name)}" data-active="0">Aktife Al</button>`}
                    </div>
                  </td>
                </tr>
              `).join('')}
          </tbody>
        </table>
      </div>
    </div>
  `;

  container.querySelector('#btn-add-project').addEventListener('click', () => openProjectModal(null, container));

  container.querySelectorAll('.btn-edit').forEach(btn => {
    btn.addEventListener('click', () => {
      const project = projects.find(p => p.id === Number(btn.dataset.id));
      if (project) openProjectModal(project, container);
    });
  });

  container.querySelectorAll('.btn-toggle').forEach(btn => {
    btn.addEventListener('click', async () => {
      const active = btn.dataset.active === '1';
      const verb = active ? 'pasife almak' : 'aktife almak';
      if (!confirm(`"${btn.dataset.name}" projesini ${verb} istiyor musunuz?`)) return;
      try {
        if (active) {
          await api.projects.delete(Number(btn.dataset.id));
        } else {
          await api.projects.update(Number(btn.dataset.id), { active: true });
        }
        adminToast(`Proje ${active ? 'pasife alındı' : 'aktife alındı'}.`, 'success');
        await renderList(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    });
  });
}

function openProjectModal(project, container) {
  const isEdit = !!project;
  const html = `
    <div class="modal-header">
      <h2>${isEdit ? 'Proje Düzenle' : 'Yeni Proje'}</h2>
      <button class="modal-close-btn" id="modal-close">✕</button>
    </div>
    <div class="modal-body">
      <div class="form-group">
        <label>Proje Adı *</label>
        <input type="text" id="f-name" class="form-input" placeholder="ör. Hat-1 Muayene" value="${isEdit ? esc(project.name) : ''}" />
      </div>
      <p id="modal-error" class="text-danger" style="display:none;"></p>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" id="modal-cancel">İptal</button>
      <button class="btn btn-primary" id="modal-save">${isEdit ? 'Kaydet' : 'Oluştur'}</button>
    </div>
  `;

  adminModal.open(html, { onClose: () => renderList(container) });

  const box = document.getElementById('admin-modal-box');
  box.querySelector('#modal-close').addEventListener('click', () => adminModal.close());
  box.querySelector('#modal-cancel').addEventListener('click', () => adminModal.close());
  box.querySelector('#modal-save').addEventListener('click', async () => {
    const errEl = box.querySelector('#modal-error');
    errEl.style.display = 'none';
    const name = box.querySelector('#f-name').value.trim();
    if (!name) { errEl.textContent = 'Proje adı boş olamaz.'; errEl.style.display = 'block'; return; }
    try {
      if (isEdit) {
        await api.projects.update(project.id, { name });
        adminToast('Proje güncellendi.', 'success');
      } else {
        await api.projects.create({ name });
        adminToast('Proje oluşturuldu.', 'success');
      }
      adminModal.close();
      await renderList(container);
    } catch (e) {
      errEl.textContent = e.message;
      errEl.style.display = 'block';
    }
  });
}

function esc(s) {
  return String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

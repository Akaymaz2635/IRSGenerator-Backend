import { api } from '../api.js';

export async function adminDefectTypesPage(container) {
  await renderList(container);
}

async function renderList(container) {
  let types;
  try {
    types = await api.defectTypes.list(true);
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Hata Tipleri</h1>
        <p>${types.length} kayıt</p>
      </div>
      <button class="btn btn-primary" id="btn-add">+ Yeni Hata Tipi</button>
    </div>

    <div class="admin-card">
      <div class="admin-table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Ad</th>
              <th>Durum</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            ${types.length === 0 ? '<tr><td colspan="4" style="text-align:center;color:var(--text-muted);padding:32px;">Hata tipi bulunamadı</td></tr>' :
              types.map(t => `
                <tr>
                  <td style="color:var(--text-muted);font-size:12px">#${t.id}</td>
                  <td>${esc(t.name)}</td>
                  <td>
                    <span class="badge ${t.active ? 'badge-active' : 'badge-inactive'}">
                      ${t.active ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td>
                    <div class="tbl-actions">
                      <button class="btn btn-sm btn-ghost btn-edit" data-id="${t.id}">Düzenle</button>
                      <button class="btn btn-sm btn-secondary btn-fields" data-id="${t.id}" data-name="${esc(t.name)}">Alanlar</button>
                      ${t.active
                        ? `<button class="btn btn-sm btn-danger btn-toggle" data-id="${t.id}" data-name="${esc(t.name)}" data-active="1">Pasife Al</button>`
                        : `<button class="btn btn-sm btn-secondary btn-toggle" data-id="${t.id}" data-name="${esc(t.name)}" data-active="0">Aktife Al</button>`}
                    </div>
                  </td>
                </tr>
              `).join('')}
          </tbody>
        </table>
      </div>
    </div>
  `;

  container.querySelector('#btn-add').addEventListener('click', () => openModal(null, container));

  container.querySelectorAll('.btn-edit').forEach(btn => {
    btn.addEventListener('click', () => {
      const t = types.find(x => x.id === Number(btn.dataset.id));
      if (t) openModal(t, container);
    });
  });

  container.querySelectorAll('.btn-fields').forEach(btn => {
    btn.addEventListener('click', () => {
      // Navigate to defect-fields page with pre-selected defect type
      sessionStorage.setItem('admin_fields_type_id', btn.dataset.id);
      sessionStorage.setItem('admin_fields_type_name', btn.dataset.name);
      location.hash = '#/defect-fields';
    });
  });

  container.querySelectorAll('.btn-toggle').forEach(btn => {
    btn.addEventListener('click', async () => {
      const active = btn.dataset.active === '1';
      const verb = active ? 'pasife almak' : 'aktife almak';
      if (!confirm(`"${btn.dataset.name}" tipini ${verb} istiyor musunuz?`)) return;
      try {
        if (active) {
          await api.defectTypes.delete(Number(btn.dataset.id));
        } else {
          await api.defectTypes.update(Number(btn.dataset.id), { active: true });
        }
        adminToast(`Hata tipi ${active ? 'pasife alındı' : 'aktife alındı'}.`, 'success');
        await renderList(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    });
  });
}

function openModal(type, container) {
  const isEdit = !!type;
  const html = `
    <div class="modal-header">
      <h2>${isEdit ? 'Hata Tipi Düzenle' : 'Yeni Hata Tipi'}</h2>
      <button class="modal-close-btn" id="modal-close">✕</button>
    </div>
    <div class="modal-body">
      <div class="form-group">
        <label>Ad *</label>
        <input type="text" id="f-name" class="form-input" placeholder="ör. Çizik" value="${isEdit ? esc(type.name) : ''}" />
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
    if (!name) { errEl.textContent = 'Ad boş olamaz.'; errEl.style.display = 'block'; return; }
    try {
      if (isEdit) {
        await api.defectTypes.update(type.id, { name });
        adminToast('Hata tipi güncellendi.', 'success');
      } else {
        await api.defectTypes.create({ name });
        adminToast('Hata tipi oluşturuldu.', 'success');
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

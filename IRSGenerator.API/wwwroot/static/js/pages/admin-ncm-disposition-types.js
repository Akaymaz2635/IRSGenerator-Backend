import { api } from '../api.js';

export async function adminNcmDispositionTypesPage(container) {
  await renderPage(container);
}

async function renderPage(container) {
  let items;
  try {
    items = await api.ncmDispositionTypes.list();
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>NCM Karar Tipleri</h1>
        <p>${items.length} kayıt</p>
      </div>
      <button class="btn btn-primary" id="btn-add">+ Yeni NCM Karar Tipi</button>
    </div>

    <div class="admin-card">
      <div class="admin-table-wrap">
        <table>
          <thead>
            <tr>
              <th>Kod</th>
              <th>Etiket</th>
              <th>Açıklama</th>
              <th>Template Dosyası</th>
              <th>Durum</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            ${items.length === 0
              ? '<tr><td colspan="7" style="text-align:center;color:var(--text-muted);padding:32px;">NCM karar tipi bulunamadı</td></tr>'
              : items.map(item => `
                <tr>
                  <td><code style="font-size:11px;">${esc(item.code)}</code></td>
                  <td>${esc(item.label)}</td>
                  <td style="color:var(--text-muted);font-size:12px;">${esc(item.description ?? '')}</td>
                  <td><code style="font-size:11px;">${esc(item.template_file_name ?? '')}</code></td>
                  <td>
                    <span class="badge ${item.active ? 'badge-active' : 'badge-inactive'}">
                      ${item.active ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td>
                    <div class="tbl-actions">
                      <button class="btn btn-sm btn-ghost btn-edit" data-id="${item.id}">Düzenle</button>
                      <button class="btn btn-sm btn-secondary btn-upload" data-id="${item.id}" data-filename="${esc(item.template_file_name)}">Şablon Yükle</button>
                      ${item.active
                        ? `<button class="btn btn-sm btn-danger btn-toggle" data-id="${item.id}" data-active="1">Pasife Al</button>`
                        : `<button class="btn btn-sm btn-secondary btn-toggle" data-id="${item.id}" data-active="0">Aktife Al</button>`}
                    </div>
                  </td>
                </tr>
              `).join('')}
          </tbody>
        </table>
      </div>
    </div>
  `;

  container.querySelector('#btn-add').addEventListener('click', () =>
    openModal(null, container)
  );

  container.querySelectorAll('.btn-edit').forEach(btn =>
    btn.addEventListener('click', () => {
      const item = items.find(x => x.id === Number(btn.dataset.id));
      if (item) openModal(item, container);
    })
  );

  container.querySelectorAll('.btn-upload').forEach(btn =>
    btn.addEventListener('click', () => {
      const fileName = btn.dataset.filename;
      if (!fileName) {
        adminToast('Önce bu karar tipine bir template dosya adı tanımlayın.', 'error');
        return;
      }
      const input = document.createElement('input');
      input.type   = 'file';
      input.accept = '.docx';
      input.onchange = async () => {
        const file = input.files[0];
        if (!file) return;
        try {
          await api.ncm.uploadTemplate(fileName, file);
          adminToast(`Şablon yüklendi: ${fileName}`, 'success');
        } catch (e) {
          adminToast('Yükleme hatası: ' + e.message, 'error');
        }
      };
      input.click();
    })
  );

  container.querySelectorAll('.btn-toggle').forEach(btn =>
    btn.addEventListener('click', async () => {
      const isActive = btn.dataset.active === '1';
      const id = Number(btn.dataset.id);
      try {
        if (isActive) await api.ncmDispositionTypes.delete(id);
        else          await api.ncmDispositionTypes.update(id, { active: true });
        adminToast(`NCM karar tipi ${isActive ? 'pasife alındı' : 'aktife alındı'}.`, 'success');
        await renderPage(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    })
  );
}

function openModal(existing, container) {
  const isEdit = !!existing;
  const html = `
    <div class="modal-header">
      <h2>${isEdit ? 'NCM Karar Tipi Düzenle' : 'Yeni NCM Karar Tipi'}</h2>
      <button class="modal-close-btn" id="modal-close">✕</button>
    </div>
    <div class="modal-body">
      ${!isEdit ? `
        <div class="form-group">
          <label>Kod * <span style="font-size:11px;color:var(--text-muted);">(sonradan değiştirilemez)</span></label>
          <input type="text" id="f-code" class="form-input" placeholder="ör. ACCEPT" />
        </div>` : `
        <div class="form-group">
          <label>Kod</label>
          <input type="text" id="f-code" class="form-input" value="${esc(existing.code)}" />
        </div>`}
      <div class="form-group">
        <label>Etiket *</label>
        <input type="text" id="f-label" class="form-input" placeholder="ör. Accept"
               value="${isEdit ? esc(existing.label) : ''}" />
      </div>
      <div class="form-group">
        <label>Açıklama</label>
        <input type="text" id="f-description" class="form-input" placeholder="ör. Parça olduğu gibi kabul edilir"
               value="${isEdit ? esc(existing.description ?? '') : ''}" />
      </div>
      <div class="form-group">
        <label>Template Dosyası <span style="font-size:11px;color:var(--text-muted);">(ör. ACCEPT.docx)</span></label>
        <input type="text" id="f-template" class="form-input" placeholder="ACCEPT.docx"
               value="${isEdit ? esc(existing.template_file_name ?? '') : ''}" />
      </div>
      ${isEdit ? `
      <div class="form-group">
        <label style="display:flex;align-items:center;gap:8px;cursor:pointer;">
          <input type="checkbox" id="f-active" ${existing.active ? 'checked' : ''} />
          Aktif
        </label>
      </div>` : ''}
      <p id="modal-error" class="text-danger" style="display:none;margin-top:8px;"></p>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" id="modal-cancel">İptal</button>
      <button class="btn btn-primary" id="modal-save">${isEdit ? 'Kaydet' : 'Oluştur'}</button>
    </div>
  `;

  adminModal.open(html, { onClose: () => renderPage(container) });

  const box = document.getElementById('admin-modal-box');
  box.querySelector('#modal-close').addEventListener('click',  () => adminModal.close());
  box.querySelector('#modal-cancel').addEventListener('click', () => adminModal.close());

  box.querySelector('#modal-save').addEventListener('click', async () => {
    const errEl = box.querySelector('#modal-error');
    errEl.style.display = 'none';

    const label              = box.querySelector('#f-label').value.trim();
    const description        = box.querySelector('#f-description').value.trim();
    const template_file_name = box.querySelector('#f-template').value.trim();

    if (!label) {
      errEl.textContent = 'Etiket boş olamaz.';
      errEl.style.display = 'block';
      return;
    }

    try {
      if (isEdit) {
        const code   = box.querySelector('#f-code').value.trim().toUpperCase();
        const active = box.querySelector('#f-active').checked;
        if (!code) { errEl.textContent = 'Kod boş olamaz.'; errEl.style.display = 'block'; return; }
        await api.ncmDispositionTypes.update(existing.id, {
          code, label, description, template_file_name, active
        });
        adminToast('NCM karar tipi güncellendi.', 'success');
      } else {
        const code = box.querySelector('#f-code').value.trim().toUpperCase();
        if (!code) {
          errEl.textContent = 'Kod boş olamaz.';
          errEl.style.display = 'block';
          return;
        }
        await api.ncmDispositionTypes.create({ code, label, description, template_file_name });
        adminToast('NCM karar tipi oluşturuldu.', 'success');
      }
      adminModal.close();
      await renderPage(container);
    } catch (e) {
      errEl.textContent = e.message;
      errEl.style.display = 'block';
    }
  });
}

function esc(s) {
  return String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

import { api } from '../api.js';

export async function adminCauseCodesPage(container) {
  await renderPage(container);
}

async function renderPage(container) {
  let items;
  try {
    items = await api.causeCodes.list();
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Cause Codes</h1>
        <p>${items.length} kayıt</p>
      </div>
      <button class="btn btn-primary" id="btn-add">+ Yeni Cause Code</button>
    </div>

    <div class="admin-card">
      <div class="admin-table-wrap">
        <table>
          <thead>
            <tr>
              <th>Kod</th>
              <th>Açıklama</th>
              <th>Durum</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            ${items.length === 0
              ? '<tr><td colspan="5" style="text-align:center;color:var(--text-muted);padding:32px;">Cause code bulunamadı</td></tr>'
              : items.map(item => `
                <tr>
                  <td><code style="font-size:11px;">${esc(item.code)}</code></td>
                  <td>${esc(item.description ?? '')}</td>
                  <td>
                    <span class="badge ${item.active ? 'badge-active' : 'badge-inactive'}">
                      ${item.active ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td>
                    <div class="tbl-actions">
                      <button class="btn btn-sm btn-ghost btn-edit" data-id="${item.id}">Düzenle</button>
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

  container.querySelectorAll('.btn-toggle').forEach(btn =>
    btn.addEventListener('click', async () => {
      const isActive = btn.dataset.active === '1';
      const id = Number(btn.dataset.id);
      try {
        if (isActive) await api.causeCodes.delete(id);
        else          await api.causeCodes.update(id, { active: true });
        adminToast(`Cause code ${isActive ? 'pasife alındı' : 'aktife alındı'}.`, 'success');
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
      <h2>${isEdit ? 'Cause Code Düzenle' : 'Yeni Cause Code'}</h2>
      <button class="modal-close-btn" id="modal-close">✕</button>
    </div>
    <div class="modal-body">
      ${!isEdit ? `
        <div class="form-group">
          <label>Kod * <span style="font-size:11px;color:var(--text-muted);">(sonradan değiştirilemez)</span></label>
          <input type="text" id="f-code" class="form-input" placeholder="ör. MACHINING_ERROR" />
        </div>` : `
        <div class="form-group">
          <label>Kod</label>
          <input type="text" id="f-code" class="form-input" value="${esc(existing.code)}" />
        </div>`}
      <div class="form-group">
        <label>Açıklama *</label>
        <input type="text" id="f-description" class="form-input" placeholder="ör. Machining Error"
               value="${isEdit ? esc(existing.description ?? '') : ''}" />
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

    const description = box.querySelector('#f-description').value.trim();

    if (!description) {
      errEl.textContent = 'Açıklama boş olamaz.';
      errEl.style.display = 'block';
      return;
    }

    try {
      if (isEdit) {
        const code   = box.querySelector('#f-code').value.trim().toUpperCase();
        const active = box.querySelector('#f-active').checked;
        if (!code) { errEl.textContent = 'Kod boş olamaz.'; errEl.style.display = 'block'; return; }
        await api.causeCodes.update(existing.id, { code, description, active });
        adminToast('Cause code güncellendi.', 'success');
      } else {
        const code = box.querySelector('#f-code').value.trim().toUpperCase();
        if (!code) {
          errEl.textContent = 'Kod boş olamaz.';
          errEl.style.display = 'block';
          return;
        }
        await api.causeCodes.create({ code, description });
        adminToast('Cause code oluşturuldu.', 'success');
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

import { api } from '../api.js';

const ROLES = [
  { value: 'inspector', label: 'Muayeneci' },
  { value: 'engineer',  label: 'Mühendis' },
  { value: 'admin',     label: 'Admin' },
];

function roleLabel(r) {
  return ROLES.find(x => x.value === r)?.label || r;
}

function roleBadge(r) {
  const cls = r === 'admin' ? 'badge-admin' : r === 'engineer' ? 'badge-engineer' : 'badge-inspector';
  return `<span class="badge ${cls}">${roleLabel(r)}</span>`;
}

export async function adminUsersPage(container) {
  await renderList(container);
}

async function renderList(container) {
  let users;
  try {
    users = await api.users.list(true);
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Kullanıcılar</h1>
        <p>${users.length} kayıt</p>
      </div>
      <button class="btn btn-primary" id="btn-add-user">+ Yeni Kullanıcı</button>
    </div>

    <div class="admin-card">
      <div class="admin-table-wrap">
        <table>
          <thead>
            <tr>
              <th>Sicil No</th>
              <th>Ad Soyad</th>
              <th>Rol</th>
              <th>Durum</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            ${users.length === 0 ? '<tr><td colspan="5" style="text-align:center;color:var(--text-muted);padding:32px;">Kullanıcı bulunamadı</td></tr>' :
              users.map(u => `
                <tr>
                  <td><code style="font-size:12px">${esc(u.employee_id)}</code></td>
                  <td>${esc(u.name)}</td>
                  <td>${roleBadge(u.role)}</td>
                  <td>
                    <span class="badge ${u.active ? 'badge-active' : 'badge-inactive'}">
                      ${u.active ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td>
                    <div class="tbl-actions">
                      <button class="btn btn-sm btn-ghost btn-edit" data-id="${u.id}">Düzenle</button>
                      ${u.active
                        ? `<button class="btn btn-sm btn-danger btn-deactivate" data-id="${u.id}" data-name="${esc(u.name)}">Pasife Al</button>`
                        : `<button class="btn btn-sm btn-secondary btn-activate" data-id="${u.id}" data-name="${esc(u.name)}">Aktife Al</button>`}
                    </div>
                  </td>
                </tr>
              `).join('')}
          </tbody>
        </table>
      </div>
    </div>
  `;

  container.querySelector('#btn-add-user').addEventListener('click', () => openUserModal(null, container));

  container.querySelectorAll('.btn-edit').forEach(btn => {
    btn.addEventListener('click', async () => {
      const user = users.find(u => u.id === Number(btn.dataset.id));
      if (user) openUserModal(user, container);
    });
  });

  container.querySelectorAll('.btn-deactivate').forEach(btn => {
    btn.addEventListener('click', async () => {
      if (!confirm(`"${btn.dataset.name}" kullanıcısını pasife almak istiyor musunuz?`)) return;
      try {
        await api.users.deactivate(Number(btn.dataset.id));
        adminToast('Kullanıcı pasife alındı.', 'success');
        await renderList(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    });
  });

  container.querySelectorAll('.btn-activate').forEach(btn => {
    btn.addEventListener('click', async () => {
      try {
        await api.users.update(Number(btn.dataset.id), { active: true });
        adminToast('Kullanıcı aktife alındı.', 'success');
        await renderList(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    });
  });
}

function openUserModal(user, container) {
  const isEdit = !!user;
  const html = `
    <div class="modal-header">
      <h2>${isEdit ? 'Kullanıcı Düzenle' : 'Yeni Kullanıcı'}</h2>
      <button class="modal-close-btn" id="modal-close">✕</button>
    </div>
    <div class="modal-body">
      ${!isEdit ? `
        <div class="form-group">
          <label>Sicil No *</label>
          <input type="text" id="f-employee-id" class="form-input" placeholder="ör. 12345" autocomplete="off" />
        </div>
      ` : `
        <div class="form-group">
          <label>Sicil No</label>
          <input type="text" class="form-input" value="${esc(user.employee_id)}" disabled />
        </div>
      `}
      <div class="form-group">
        <label>Ad Soyad *</label>
        <input type="text" id="f-name" class="form-input" placeholder="ör. Ahmet Yılmaz" value="${isEdit ? esc(user.name) : ''}" />
      </div>
      <div class="form-group">
        <label>Rol *</label>
        <select id="f-role" class="form-select">
          ${ROLES.map(r => `<option value="${r.value}" ${isEdit && user.role === r.value ? 'selected' : ''}>${r.label}</option>`).join('')}
        </select>
      </div>
      <div class="form-group">
        <label>${isEdit ? 'Şifre' : 'Şifre *'}</label>
        <input type="text" id="f-password" class="form-input"
               placeholder="${isEdit ? 'Boş bırakılırsa şifre değişmez' : 'Şifre belirleyin'}"
               value="${isEdit ? esc(user.password || '') : ''}"
               autocomplete="off" />
        ${isEdit ? '<div class="form-hint">Şifreyi değiştirmek istemiyorsanız boş bırakın.</div>' : ''}
      </div>
      ${isEdit ? '' : `
      <div class="form-group">
        <label>Şifre Tekrar *</label>
        <input type="text" id="f-password2" class="form-input"
               placeholder="Şifreyi tekrar girin" autocomplete="off" />
      </div>
      `}
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
    const errEl    = box.querySelector('#modal-error');
    errEl.style.display = 'none';

    const name     = box.querySelector('#f-name').value.trim();
    const role     = box.querySelector('#f-role').value;
    const password = box.querySelector('#f-password').value;
    const password2 = box.querySelector('#f-password2')?.value ?? null;

    if (!name) { errEl.textContent = 'Ad Soyad boş olamaz.'; errEl.style.display = 'block'; return; }

    if (!isEdit) {
      if (!password) { errEl.textContent = 'Şifre boş olamaz.'; errEl.style.display = 'block'; return; }
      if (password !== password2) { errEl.textContent = 'Şifreler eşleşmiyor.'; errEl.style.display = 'block'; return; }
    }

    try {
      if (isEdit) {
        const payload = { name, role };
        if (password) payload.password = password;
        await api.users.update(user.id, payload);
        adminToast('Kullanıcı güncellendi.', 'success');
      } else {
        const employeeId = box.querySelector('#f-employee-id').value.trim();
        if (!employeeId) { errEl.textContent = 'Sicil No boş olamaz.'; errEl.style.display = 'block'; return; }
        await api.users.create({ employee_id: employeeId, name, role, password });
        adminToast('Kullanıcı oluşturuldu.', 'success');
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

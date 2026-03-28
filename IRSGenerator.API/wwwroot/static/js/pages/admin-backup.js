import { api } from '../api.js';

export async function adminBackupPage(container) {
  await render(container);
}

async function render(container) {
  let data, config;
  try {
    [data, config] = await Promise.all([
      api.backup.getAll(),
      api.systemConfig.get(),
    ]);
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Güvenlik &amp; Yedekleme</h1>
        <p>Veritabanı şifresi ve yedek yönetimi</p>
      </div>
    </div>

    <!-- ── Veritabanı Şifresi ── -->
    <div class="admin-card" style="padding:24px;max-width:560px;margin-bottom:24px;">
      <h2 style="font-size:15px;font-weight:600;margin-bottom:4px;">Veritabanı Şifresi</h2>
      <p style="font-size:13px;color:var(--text-muted);margin-bottom:20px;">
        Access veritabanı dosyasını şifreler. Şifre <code>config.json</code>'a kaydedilir.
        ${config.db_password_set
          ? '<span style="color:var(--success,#10b981);font-weight:600;">● Şifre tanımlı</span>'
          : '<span style="color:var(--text-muted);">● Şifre tanımlı değil</span>'}
      </p>

      <div class="form-group">
        <label>Mevcut Şifre ${config.db_password_set ? '' : '<span style="color:var(--text-muted);font-size:11px;">(tanımlı değilse boş bırakın)</span>'}</label>
        <input type="password" id="pw-old" class="form-input" autocomplete="current-password"
               placeholder="${config.db_password_set ? 'Mevcut şifre' : 'Boş bırakın'}" />
      </div>
      <div class="form-group">
        <label>Yeni Şifre <span style="color:var(--text-muted);font-size:11px;">(kaldırmak için boş bırakın)</span></label>
        <input type="password" id="pw-new" class="form-input" autocomplete="new-password"
               placeholder="Yeni şifre" />
      </div>
      <div class="form-group">
        <label>Yeni Şifre Tekrar</label>
        <input type="password" id="pw-new2" class="form-input" autocomplete="new-password"
               placeholder="Yeni şifre tekrar" />
      </div>

      <div style="background:#fef3c7;border:1px solid #f59e0b;border-radius:6px;padding:10px 14px;
                  font-size:12px;color:#92400e;margin-bottom:16px;">
        ⚠️ Şifre değiştirme sırasında başka kullanıcının aktif olmaması önerilir.
        İşlem tüm bağlantılar serbest kalana kadar bekler.
      </div>

      <p id="pw-error"   class="text-danger" style="display:none;margin-bottom:10px;"></p>
      <p id="pw-success" style="display:none;margin-bottom:10px;font-size:13px;color:var(--success,#10b981);"></p>
      <button class="btn btn-primary" id="btn-pw-save">Şifreyi Değiştir</button>
    </div>

    <!-- ── Yedekleme ── -->
    <div class="admin-card" style="padding:24px;max-width:760px;">
      <h2 style="font-size:15px;font-weight:600;margin-bottom:16px;">Yedekleme</h2>

      <div style="display:flex;gap:12px;align-items:flex-end;margin-bottom:20px;flex-wrap:wrap;">
        <div class="form-group" style="margin:0;flex:1;min-width:280px;">
          <label>Yedek Klasörü</label>
          <input type="text" id="backup-folder" class="form-input"
                 value="${esc(data.folder)}"
                 placeholder="ör. C:\\QualiSight\\Backups" />
        </div>
        <button class="btn btn-secondary" id="btn-folder-save" style="white-space:nowrap;">Klasörü Kaydet</button>
        <button class="btn btn-primary"   id="btn-backup-now"  style="white-space:nowrap;">▶ Şimdi Yedekle</button>
      </div>

      <p id="backup-error"   class="text-danger" style="display:none;margin-bottom:10px;"></p>
      <p id="backup-success" style="display:none;margin-bottom:10px;font-size:13px;color:var(--success,#10b981);"></p>

      <div id="backup-list">
        ${renderBackupTable(data.backups)}
      </div>
    </div>
  `;

  // ── Şifre değiştirme ──
  container.querySelector('#btn-pw-save').addEventListener('click', async () => {
    const errEl  = container.querySelector('#pw-error');
    const okEl   = container.querySelector('#pw-success');
    const oldPw  = container.querySelector('#pw-old').value;
    const newPw  = container.querySelector('#pw-new').value;
    const newPw2 = container.querySelector('#pw-new2').value;

    errEl.style.display = 'none';
    okEl.style.display  = 'none';

    if (newPw !== newPw2) {
      errEl.textContent   = 'Yeni şifreler eşleşmiyor.';
      errEl.style.display = 'block';
      return;
    }

    const btn = container.querySelector('#btn-pw-save');
    btn.disabled     = true;
    btn.textContent  = 'İşleniyor…';
    try {
      const res = await api.backup.setPassword(oldPw, newPw);
      okEl.textContent   = '✓ ' + res.detail;
      okEl.style.display = 'block';
      container.querySelector('#pw-old').value  = '';
      container.querySelector('#pw-new').value  = '';
      container.querySelector('#pw-new2').value = '';
      adminToast('Şifre değiştirildi.', 'success');
      // Sayfayı yenile (db_password_set badge güncellenir)
      await render(container);
    } catch (e) {
      errEl.textContent   = e.message;
      errEl.style.display = 'block';
    } finally {
      btn.disabled    = false;
      btn.textContent = 'Şifreyi Değiştir';
    }
  });

  // ── Klasör kaydet ──
  container.querySelector('#btn-folder-save').addEventListener('click', async () => {
    const folder = container.querySelector('#backup-folder').value.trim();
    const errEl  = container.querySelector('#backup-error');
    const okEl   = container.querySelector('#backup-success');
    errEl.style.display = 'none';
    okEl.style.display  = 'none';
    try {
      await api.backup.setFolder(folder);
      okEl.textContent   = '✓ Yedek klasörü kaydedildi.';
      okEl.style.display = 'block';
      adminToast('Klasör güncellendi.', 'success');
    } catch (e) {
      errEl.textContent   = e.message;
      errEl.style.display = 'block';
    }
  });

  // ── Şimdi yedekle ──
  container.querySelector('#btn-backup-now').addEventListener('click', async () => {
    const errEl = container.querySelector('#backup-error');
    const okEl  = container.querySelector('#backup-success');
    const btn   = container.querySelector('#btn-backup-now');
    errEl.style.display = 'none';
    okEl.style.display  = 'none';
    btn.disabled    = true;
    btn.textContent = 'Yedekleniyor…';
    try {
      const info = await api.backup.create();
      okEl.textContent   = `✓ Yedek oluşturuldu: ${info.filename} (${info.size_kb} KB)`;
      okEl.style.display = 'block';
      adminToast('Yedek oluşturuldu.', 'success');
      // Listeyi yenile
      const fresh = await api.backup.getAll();
      container.querySelector('#backup-list').innerHTML = renderBackupTable(fresh.backups);
      bindDeleteButtons(container);
    } catch (e) {
      errEl.textContent   = e.message;
      errEl.style.display = 'block';
    } finally {
      btn.disabled    = false;
      btn.textContent = '▶ Şimdi Yedekle';
    }
  });

  bindDeleteButtons(container);
}

function renderBackupTable(backups) {
  if (!backups || backups.length === 0) {
    return `<div style="text-align:center;color:var(--text-muted);padding:24px;font-size:13px;">
              Henüz yedek oluşturulmamış.
            </div>`;
  }
  return `
    <div class="admin-table-wrap">
      <table>
        <thead>
          <tr>
            <th>Dosya Adı</th>
            <th>Tarih</th>
            <th>Boyut</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          ${backups.map(b => `
            <tr>
              <td><code style="font-size:12px;">${esc(b.filename)}</code></td>
              <td style="color:var(--text-muted);font-size:13px;">${esc(b.created_at)}</td>
              <td style="color:var(--text-muted);font-size:13px;">${b.size_kb} KB</td>
              <td>
                <button class="btn btn-sm btn-danger btn-del-backup"
                        data-filename="${esc(b.filename)}">Sil</button>
              </td>
            </tr>
          `).join('')}
        </tbody>
      </table>
    </div>
  `;
}

function bindDeleteButtons(container) {
  container.querySelectorAll('.btn-del-backup').forEach(btn => {
    btn.addEventListener('click', async () => {
      if (!confirm(`"${btn.dataset.filename}" yedeğini silmek istiyor musunuz?`)) return;
      try {
        await api.backup.delete(btn.dataset.filename);
        adminToast('Yedek silindi.', 'success');
        const fresh = await api.backup.getAll();
        container.querySelector('#backup-list').innerHTML = renderBackupTable(fresh.backups);
        bindDeleteButtons(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    });
  });
}

function esc(s) {
  return String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

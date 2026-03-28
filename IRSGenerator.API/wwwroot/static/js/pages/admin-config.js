import { api } from '../api.js';

export async function adminConfigPage(container) {
  await renderConfig(container);
}

async function renderConfig(container) {
  let config;
  try {
    config = await api.systemConfig.get();
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Sistem Yapılandırması</h1>
        <p>Dosya yolları ve veritabanı ayarları</p>
      </div>
    </div>

    <div class="admin-card" style="padding:24px;max-width:680px;">

      <div class="config-section">
        <h3>Veritabanı</h3>
        <div class="form-group">
          <label>Veritabanı Yolu</label>
          <input type="text" class="form-input" value="${esc(config.database_path || '')}" disabled />
          <div class="form-hint">Bu yol config.json dosyasından okunur ve uygulama içinden değiştirilemez.</div>
        </div>
      </div>

      <div class="config-section">
        <h3>Fotoğraf Klasörü</h3>
        <div class="form-group">
          <label>Fotoğraf Kök Dizini</label>
          <input type="text" id="f-photo-root" class="form-input"
                 placeholder="ör. C:\\QualiSight\\Photos"
                 value="${esc(config.photo_root_folder || '')}" />
          <div class="form-hint path-preview" id="photo-preview">
            Boş bırakılırsa uygulama dizinindeki "Photos" klasörü kullanılır.
          </div>
        </div>
      </div>

      <div class="config-section">
        <h3>Rapor Klasörü</h3>
        <div class="form-group">
          <label>Rapor Kök Dizini</label>
          <input type="text" id="f-report-root" class="form-input"
                 placeholder="ör. C:\\QualiSight\\Reports"
                 value="${esc(config.report_root_folder || '')}" />
          <div class="form-hint path-preview" id="report-preview">
            Boş bırakılırsa uygulama dizinindeki "Reports" klasörü kullanılır.
          </div>
        </div>
      </div>

      <p id="config-error" class="text-danger" style="display:none;margin-bottom:12px;"></p>
      <p id="config-success" style="display:none;margin-bottom:12px;font-size:13px;color:var(--success);">✓ Yapılandırma kaydedildi.</p>

      <div style="display:flex;gap:10px;">
        <button class="btn btn-primary" id="btn-save">Kaydet</button>
        <button class="btn btn-ghost"   id="btn-reset">Sıfırla</button>
      </div>
    </div>
  `;

  const photoInput  = container.querySelector('#f-photo-root');
  const reportInput = container.querySelector('#f-report-root');
  const errEl       = container.querySelector('#config-error');
  const okEl        = container.querySelector('#config-success');

  container.querySelector('#btn-save').addEventListener('click', async () => {
    errEl.style.display = 'none';
    okEl.style.display  = 'none';

    const photoRoot  = photoInput.value.trim() || null;
    const reportRoot = reportInput.value.trim() || null;

    try {
      await api.systemConfig.update({
        photo_root_folder:  photoRoot,
        report_root_folder: reportRoot,
      });
      okEl.style.display = 'block';
      adminToast('Yapılandırma kaydedildi.', 'success');
    } catch (e) {
      errEl.textContent   = e.message;
      errEl.style.display = 'block';
      adminToast('Kayıt başarısız: ' + e.message, 'error');
    }
  });

  container.querySelector('#btn-reset').addEventListener('click', async () => {
    if (!confirm('Yapılandırmayı son kaydedilen değerlere döndürmek istiyor musunuz?')) return;
    await renderConfig(container);
  });
}

function esc(s) {
  return String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

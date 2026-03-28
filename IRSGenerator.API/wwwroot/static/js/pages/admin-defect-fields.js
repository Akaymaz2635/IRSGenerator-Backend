import { api } from '../api.js';

const STANDARD_FIELDS = [
  { field_name: 'depth',  label: 'Derinlik', unit: 'mm', field_type: 'number' },
  { field_name: 'width',  label: 'Genişlik', unit: 'mm', field_type: 'number' },
  { field_name: 'length', label: 'Uzunluk',  unit: 'mm', field_type: 'number' },
  { field_name: 'radius', label: 'Yarıçap',  unit: 'mm', field_type: 'number' },
  { field_name: 'angle',  label: 'Açı',      unit: '°',  field_type: 'number' },
  { field_name: 'color',  label: 'Renk',     unit: '',   field_type: 'text'   },
];
const STANDARD_NAMES = new Set(STANDARD_FIELDS.map(f => f.field_name));

export async function adminDefectFieldsPage(container) {
  let defectTypes;
  try {
    defectTypes = await api.defectTypes.list(true);
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  const preId      = sessionStorage.getItem('admin_fields_type_id');
  sessionStorage.removeItem('admin_fields_type_id');
  sessionStorage.removeItem('admin_fields_type_name');
  const selectedId = preId ? Number(preId) : (defectTypes[0]?.id || null);

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Hata Alanları</h1>
        <p>Her hata tipine ait ölçüm alanlarını yönetin</p>
      </div>
    </div>

    <div class="admin-card" style="padding:20px;margin-bottom:20px;">
      <div class="form-group" style="margin-bottom:0;">
        <label>Hata Tipi</label>
        <select id="type-selector" class="form-select" style="max-width:320px;">
          ${defectTypes.map(t => `<option value="${t.id}" ${t.id === selectedId ? 'selected' : ''}>${esc(t.name)}${t.active ? '' : ' (pasif)'}</option>`).join('')}
        </select>
      </div>
    </div>

    <div id="fields-section"></div>
  `;

  const selector = container.querySelector('#type-selector');
  const section  = container.querySelector('#fields-section');

  async function loadFields() {
    const typeId   = Number(selector.value);
    const typeName = defectTypes.find(t => t.id === typeId)?.name || '';
    await renderFieldsPage(section, typeId, typeName);
  }

  selector.addEventListener('change', loadFields);
  if (selectedId) await loadFields();
}

// ── Ana sayfa ──────────────────────────────────────────────────────────────────
async function renderFieldsPage(section, typeId, typeName) {
  section.innerHTML = '<div class="admin-loading">Yükleniyor…</div>';
  let existing;
  try {
    existing = await api.defectFields.list(typeId);
  } catch (e) {
    section.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  const existingMap  = Object.fromEntries(existing.map(f => [f.field_name, f]));
  const customFields = existing.filter(f => !STANDARD_NAMES.has(f.field_name));

  section.innerHTML = `
    <!-- Standart alanlar -->
    <div class="admin-card" style="padding:20px;margin-bottom:20px;">
      <h2 style="font-size:15px;font-weight:600;margin-bottom:4px;">${esc(typeName)} — Standart Alanlar</h2>
      <p style="font-size:12px;color:var(--text-muted);margin-bottom:16px;">Checkbox ile aktif/pasif yapın, birim düzenleyin.</p>
      <table style="width:100%;border-collapse:collapse;">
        <thead>
          <tr style="border-bottom:2px solid var(--border);">
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Aktif</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Alan</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Birim</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Zorunlu</th>
          </tr>
        </thead>
        <tbody id="std-tbody">
          ${STANDARD_FIELDS.map((sf, i) => {
            const ex = existingMap[sf.field_name];
            return `
              <tr style="border-bottom:1px solid var(--border);" data-index="${i}">
                <td style="padding:10px 12px;">
                  <input type="checkbox" class="cb-active" ${ex ? 'checked' : ''} style="width:16px;height:16px;cursor:pointer;" />
                </td>
                <td style="padding:10px 12px;font-weight:500;">${esc(sf.label)}</td>
                <td style="padding:10px 12px;">
                  <input type="text" class="unit-input form-input" value="${esc(ex ? (ex.unit ?? sf.unit) : sf.unit)}"
                         style="width:70px;padding:4px 8px;font-size:13px;" />
                </td>
                <td style="padding:10px 12px;">
                  <input type="checkbox" class="cb-required" ${ex?.required ? 'checked' : ''} ${!ex ? 'disabled' : ''}
                         style="width:16px;height:16px;cursor:pointer;" />
                </td>
              </tr>`;
          }).join('')}
        </tbody>
      </table>
      <div style="margin-top:20px;display:flex;gap:12px;align-items:center;">
        <button class="btn btn-primary" id="btn-save-std">Kaydet</button>
        <span id="std-save-status" style="font-size:13px;color:var(--text-muted);"></span>
      </div>
    </div>

    <!-- Özel alanlar -->
    <div class="admin-card" style="padding:20px;">
      <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:16px;">
        <div>
          <h2 style="font-size:15px;font-weight:600;margin-bottom:4px;">${esc(typeName)} — Özel Alanlar</h2>
          <p style="font-size:12px;color:var(--text-muted);">Bu hata tipine özel ek ölçüm alanları ekleyin.</p>
        </div>
        <button class="btn btn-primary btn-sm" id="btn-add-custom">+ Yeni Alan</button>
      </div>
      <div id="custom-fields-list"></div>
    </div>
  `;

  // ── Standart alanlar: active toggle ────────────────────────────────────────
  section.querySelectorAll('#std-tbody tr[data-index]').forEach(tr => {
    const cbActive   = tr.querySelector('.cb-active');
    const cbRequired = tr.querySelector('.cb-required');
    cbActive.addEventListener('change', () => {
      cbRequired.checked  = cbActive.checked ? cbRequired.checked : false;
      cbRequired.disabled = !cbActive.checked;
    });
  });

  // ── Standart alanlar: kaydet ───────────────────────────────────────────────
  section.querySelector('#btn-save-std').addEventListener('click', async () => {
    const btn      = section.querySelector('#btn-save-std');
    const statusEl = section.querySelector('#std-save-status');
    btn.disabled = true;
    statusEl.textContent = 'Kaydediliyor…';
    statusEl.style.color = 'var(--text-muted)';

    try {
      const trs = section.querySelectorAll('#std-tbody tr[data-index]');
      for (let i = 0; i < STANDARD_FIELDS.length; i++) {
        const sf      = STANDARD_FIELDS[i];
        const tr      = trs[i];
        const active   = tr.querySelector('.cb-active').checked;
        const required = tr.querySelector('.cb-required').checked;
        const unit     = tr.querySelector('.unit-input').value.trim() || null;
        const exist    = existingMap[sf.field_name];

        if (active) {
          const payload = { defect_type_id: typeId, field_name: sf.field_name, label: sf.label,
                            field_type: sf.field_type, required, unit, sort_order: i };
          if (exist) {
            await api.defectFields.update(exist.id, payload);
          } else {
            const created = await api.defectFields.create(payload);
            existingMap[sf.field_name] = created;
          }
        } else if (exist) {
          await api.defectFields.delete(exist.id);
          delete existingMap[sf.field_name];
        }
      }
      statusEl.textContent = '✓ Kaydedildi';
      statusEl.style.color = 'var(--success, #10b981)';
    } catch (e) {
      statusEl.textContent = 'Hata: ' + e.message;
      statusEl.style.color = 'var(--danger)';
    } finally {
      btn.disabled = false;
    }
  });

  // ── Özel alanlar ──────────────────────────────────────────────────────────
  function renderCustomList(fields) {
    const listEl = section.querySelector('#custom-fields-list');
    if (!fields.length) {
      listEl.innerHTML = '<p style="font-size:13px;color:var(--text-muted);padding:8px 0;">Henüz özel alan eklenmemiş.</p>';
      return;
    }
    listEl.innerHTML = `
      <table style="width:100%;border-collapse:collapse;">
        <thead>
          <tr style="border-bottom:2px solid var(--border);">
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Etiket</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Alan Adı</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Tip</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Birim</th>
            <th style="text-align:left;padding:8px 12px;font-size:12px;color:var(--text-muted);">Zorunlu</th>
            <th style="padding:8px 12px;"></th>
          </tr>
        </thead>
        <tbody>
          ${fields.map(f => `
            <tr style="border-bottom:1px solid var(--border);" data-id="${f.id}">
              <td style="padding:10px 12px;font-weight:500;">${esc(f.label)}</td>
              <td style="padding:10px 12px;font-family:monospace;font-size:12px;color:var(--text-muted);">${esc(f.field_name)}</td>
              <td style="padding:10px 12px;">${esc(f.field_type)}</td>
              <td style="padding:10px 12px;">${esc(f.unit || '—')}</td>
              <td style="padding:10px 12px;">${f.required ? '✓' : '—'}</td>
              <td style="padding:10px 12px;">
                <div style="display:flex;gap:6px;">
                  <button class="btn btn-ghost btn-sm btn-edit-custom" data-id="${f.id}">Düzenle</button>
                  <button class="btn btn-danger btn-sm btn-del-custom" data-id="${f.id}">Sil</button>
                </div>
              </td>
            </tr>`).join('')}
        </tbody>
      </table>`;

    listEl.querySelectorAll('.btn-edit-custom').forEach(btn => {
      btn.addEventListener('click', () => {
        const f = fields.find(x => x.id === Number(btn.dataset.id));
        if (f) openCustomModal(f, fields);
      });
    });

    listEl.querySelectorAll('.btn-del-custom').forEach(btn => {
      btn.addEventListener('click', async () => {
        if (!confirm('Bu özel alanı silmek istediğinizden emin misiniz?')) return;
        try {
          await api.defectFields.delete(Number(btn.dataset.id));
          const idx = fields.findIndex(x => x.id === Number(btn.dataset.id));
          if (idx !== -1) fields.splice(idx, 1);
          renderCustomList(fields);
        } catch (e) {
          alert('Silinemedi: ' + e.message);
        }
      });
    });
  }

  function openCustomModal(existing, fieldsList) {
    const isEdit = !!existing;
    window.adminModal.open(`
      <div class="modal-header">
        <h2>${isEdit ? 'Özel Alanı Düzenle' : 'Yeni Özel Alan'}</h2>
        <button class="modal-close-btn" id="modal-close">✕</button>
      </div>
      <div class="modal-body">
        <div class="form-row">
          <div class="form-group">
            <label>Etiket *</label>
            <input type="text" id="f-label" class="form-input" value="${esc(existing?.label || '')}" placeholder="ör. Sertlik" />
          </div>
          <div class="form-group">
            <label>Alan Adı * <span style="font-size:11px;color:var(--text-muted);">(küçük harf, alt çizgi)</span></label>
            <input type="text" id="f-name" class="form-input" value="${esc(existing?.field_name || '')}"
                   placeholder="ör. hardness" ${isEdit ? 'disabled' : ''} />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label>Tip</label>
            <select id="f-type" class="form-select">
              <option value="number"   ${existing?.field_type === 'number'   ? 'selected' : ''}>Sayı (number)</option>
              <option value="text"     ${existing?.field_type === 'text'     ? 'selected' : ''}>Metin (text)</option>
            </select>
          </div>
          <div class="form-group">
            <label>Birim</label>
            <input type="text" id="f-unit" class="form-input" value="${esc(existing?.unit || '')}" placeholder="ör. HRC, MPa" />
          </div>
        </div>
        <div class="form-group">
          <label class="form-checkbox">
            <input type="checkbox" id="f-required" ${existing?.required ? 'checked' : ''} />
            Zorunlu alan
          </label>
        </div>
        <p id="modal-err" style="color:var(--danger);font-size:12.5px;display:none;margin-top:4px;"></p>
      </div>
      <div class="modal-footer">
        <button class="btn btn-secondary" id="modal-cancel">İptal</button>
        <button class="btn btn-primary"   id="modal-save">${isEdit ? 'Güncelle' : 'Ekle'}</button>
      </div>
    `);

    document.getElementById('modal-close').addEventListener('click',  () => window.adminModal.close());
    document.getElementById('modal-cancel').addEventListener('click', () => window.adminModal.close());

    document.getElementById('modal-save').addEventListener('click', async () => {
      const label    = document.getElementById('f-label').value.trim();
      const name     = isEdit ? existing.field_name : document.getElementById('f-name').value.trim().toLowerCase();
      const type     = document.getElementById('f-type').value;
      const unit     = document.getElementById('f-unit').value.trim() || null;
      const required = document.getElementById('f-required').checked;
      const errEl    = document.getElementById('modal-err');

      errEl.style.display = 'none';
      if (!label) { errEl.textContent = 'Etiket zorunludur.'; errEl.style.display = 'block'; return; }
      if (!isEdit) {
        if (!name) { errEl.textContent = 'Alan adı zorunludur.'; errEl.style.display = 'block'; return; }
        if (!/^[a-z0-9_]+$/.test(name)) { errEl.textContent = 'Alan adı sadece küçük harf, rakam ve _ içerebilir.'; errEl.style.display = 'block'; return; }
        if (STANDARD_NAMES.has(name)) { errEl.textContent = 'Bu ad standart bir alan adıdır, farklı bir ad seçin.'; errEl.style.display = 'block'; return; }
        if (fieldsList.some(f => f.field_name === name)) { errEl.textContent = 'Bu alan adı zaten kullanılıyor.'; errEl.style.display = 'block'; return; }
      }

      const saveBtn = document.getElementById('modal-save');
      saveBtn.disabled = true;
      try {
        if (isEdit) {
          const updated = await api.defectFields.update(existing.id, { label, field_type: type, unit, required });
          const idx = fieldsList.findIndex(f => f.id === existing.id);
          if (idx !== -1) fieldsList[idx] = updated;
        } else {
          const created = await api.defectFields.create({
            defect_type_id: typeId, field_name: name, label,
            field_type: type, unit, required,
            sort_order: STANDARD_FIELDS.length + fieldsList.length,
          });
          fieldsList.push(created);
        }
        window.adminModal.close();
        renderCustomList(fieldsList);
      } catch (e) {
        errEl.textContent = e.message;
        errEl.style.display = 'block';
        saveBtn.disabled = false;
      }
    });
  }

  section.querySelector('#btn-add-custom').addEventListener('click', () => {
    openCustomModal(null, customFields);
  });

  renderCustomList(customFields);
}

function esc(s) {
  return String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

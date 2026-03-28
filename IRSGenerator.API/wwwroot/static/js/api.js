const BASE_URL = '';

// ── Kaydetme göstergesi ───────────────────────────────────────────────────────
let _pendingWrites = 0;
let _hideBarTimer  = null;

function _savingBar() {
  let el = document.getElementById('saving-bar');
  if (!el) {
    el = document.createElement('div');
    el.id        = 'saving-bar';
    el.innerHTML = '<span class="sb-dot"></span><span class="sb-text"></span>';
    document.body.appendChild(el);
  }
  return el;
}

function _writeBegan() {
  _pendingWrites++;
  clearTimeout(_hideBarTimer);
  const el = _savingBar();
  el.querySelector('.sb-text').textContent = 'Kaydediliyor…';
  el.className = 'saving visible';
}

function _writeEnded(ok, errMsg) {
  _pendingWrites = Math.max(0, _pendingWrites - 1);
  if (_pendingWrites > 0) return;
  const el = _savingBar();
  if (ok) {
    el.querySelector('.sb-text').textContent = 'Kaydedildi ✓';
    el.className = 'saved visible';
    _hideBarTimer = setTimeout(() => { el.className = ''; }, 2200);
  } else {
    el.querySelector('.sb-text').textContent = errMsg || 'Kayıt başarısız!';
    el.className = 'save-error visible';
    _hideBarTimer = setTimeout(() => { el.className = ''; }, 6000);
  }
}

// ─────────────────────────────────────────────────────────────────────────────

async function request(method, path, body = null, isFormData = false) {
  const isWrite = ['POST', 'PUT', 'PATCH', 'DELETE'].includes(method);
  if (isWrite) _writeBegan();

  const opts = { method, headers: {} };
  if (body !== null) {
    if (isFormData) {
      opts.body = body;
    } else {
      opts.headers['Content-Type'] = 'application/json';
      opts.body = JSON.stringify(body);
    }
  }

  try {
    const res = await fetch(BASE_URL + path, opts);
    if (res.status === 204) {
      if (isWrite) _writeEnded(true);
      return null;
    }
    const data = await res.json().catch(() => ({}));
    if (!res.ok) {
      const msg = data?.detail || `HTTP ${res.status}`;
      const err = new Error(typeof msg === 'string' ? msg : JSON.stringify(msg));
      if (isWrite) _writeEnded(false, err.message);
      throw err;
    }
    if (isWrite) _writeEnded(true);
    return data;
  } catch (e) {
    if (isWrite && !(e instanceof Error && e.message.startsWith('HTTP'))) {
      _writeEnded(false, 'Sunucuya ulaşılamadı');
    }
    throw e;
  }
}

export const api = {
  auth: {
    login: (sicil, password) => request('POST', '/api/auth/login', { sicil, password }),
  },

  projects: {
    list: (all = false) => request('GET', `/api/projects${all ? '?all=true' : ''}`),
    get:  (id)   => request('GET', `/api/projects/${id}`),
    create: (data) => request('POST', '/api/projects', data),
    update: (id, data) => request('PUT', `/api/projects/${id}`, data),
    delete: (id) => request('DELETE', `/api/projects/${id}`),
  },

  defectTypes: {
    list: (all = false) => request('GET', `/api/defect-types${all ? '?all=true' : ''}`),
    get:  (id)   => request('GET', `/api/defect-types/${id}`),
    create: (data) => request('POST', '/api/defect-types', data),
    update: (id, data) => request('PUT', `/api/defect-types/${id}`, data),
    delete: (id) => request('DELETE', `/api/defect-types/${id}`),
  },

  defectFields: {
    list: (defect_type_id) => request('GET', `/api/defect-fields?defect_type_id=${defect_type_id}`),
    get:  (id)   => request('GET', `/api/defect-fields/${id}`),
    create: (data) => request('POST', '/api/defect-fields', data),
    update: (id, data) => request('PUT', `/api/defect-fields/${id}`, data),
    delete: (id) => request('DELETE', `/api/defect-fields/${id}`),
  },

  users: {
    list: (all = true) => request('GET', `/api/users?all=${all}`),
    get:  (id)   => request('GET', `/api/users/${id}`),
    create: (data) => request('POST', '/api/users', data),
    update: (id, data) => request('PUT', `/api/users/${id}`, data),
    deactivate: (id) => request('DELETE', `/api/users/${id}`),
  },

  systemConfig: {
    get:    ()     => request('GET', '/api/system-config'),
    update: (data) => request('PUT', '/api/system-config', data),
  },

  backup: {
    getAll:      ()         => request('GET',    '/api/backup'),
    create:      ()         => request('POST',   '/api/backup'),
    delete:      (filename) => request('DELETE', `/api/backup/${encodeURIComponent(filename)}`),
    setFolder:   (folder)   => request('PUT',    '/api/backup/folder', { folder }),
    setPassword: (oldPassword, newPassword) =>
      request('POST', '/api/backup/set-password', { oldPassword, newPassword }),
  },

  inspections: {
    list: (params = {}) => {
      const qs = new URLSearchParams();
      if (params.status)     qs.set('status', params.status);
      if (params.project_id) qs.set('project_id', String(params.project_id));
      if (params.search)     qs.set('search', params.search);
      const query = qs.toString();
      return request('GET', `/api/inspections${query ? '?' + query : ''}`);
    },
    get:    (id)   => request('GET', `/api/inspections/${id}`),
    create: (data) => request('POST', '/api/inspections', data),
    update: (id, data) => request('PUT', `/api/inspections/${id}`, data),
    delete: (id)   => request('DELETE', `/api/inspections/${id}`),
  },

  defects: {
    list:   (inspection_id) => request('GET', `/api/defects?inspection_id=${inspection_id}`),
    get:    (id)   => request('GET', `/api/defects/${id}`),
    create: (data) => request('POST', '/api/defects', data),
    update: (id, data) => request('PATCH', `/api/defects/${id}`, data),
    delete: (id)   => request('DELETE', `/api/defects/${id}`),
  },

  dispositions: {
    create: (data) => request('POST', '/api/dispositions', data),
    list:   (defect_id) => request('GET', `/api/dispositions?defect_id=${defect_id}`),
    delete: (id)   => request('DELETE', `/api/dispositions/${id}`),
  },

  dispositionTypes: {
    list: (all = false) => request('GET', `/api/disposition-types${all ? '?all=true' : ''}`),
    get:  (id)   => request('GET', `/api/disposition-types/${id}`),
    create: (data) => request('POST', '/api/disposition-types', data),
    update: (id, data) => request('PUT', `/api/disposition-types/${id}`, data),
    delete: (id) => request('DELETE', `/api/disposition-types/${id}`),
  },

  dispositionTransitions: {
    list: (from_code) => {
      const qs = from_code !== undefined ? `?from_code=${encodeURIComponent(from_code ?? 'null')}` : '';
      return request('GET', `/api/disposition-transitions${qs}`);
    },
    allowed: (current_code) => {
      const qs = current_code != null ? `?current_code=${encodeURIComponent(current_code)}` : '';
      return request('GET', `/api/disposition-transitions/allowed${qs}`);
    },
    bulkSet: (from_code, to_codes) =>
      request('POST', '/api/disposition-transitions/bulk-set', { from_code, to_codes }),
  },

  photos: {
    list: (params = {}) => {
      const qs = new URLSearchParams();
      if (params.inspection_id != null) qs.set('inspection_id', String(params.inspection_id));
      if (params.defect_id     != null) qs.set('defect_id',     String(params.defect_id));
      return request('GET', `/api/photos?${qs.toString()}`);
    },
    get:      (id)   => request('GET', `/api/photos/${id}`),
    upload:   (inspection_id, file, defect_ids = []) => {
      const fd = new FormData();
      fd.append('file', file, file.name || 'photo.jpg');
      const qs = new URLSearchParams({ inspection_id: String(inspection_id) });
      const ids = Array.isArray(defect_ids) ? defect_ids : (defect_ids ? [defect_ids] : []);
      ids.forEach(id => qs.append('defect_ids', String(id)));
      return request('POST', `/api/photos?${qs.toString()}`, fd, true);
    },
    setDefects: (id, defect_ids) => request('PUT', `/api/photos/${id}/defects`, defect_ids),
    fileUrl:    (id) => `${BASE_URL}/api/photos/${id}/file`,
    delete:     (id) => request('DELETE', `/api/photos/${id}`),
  },
};

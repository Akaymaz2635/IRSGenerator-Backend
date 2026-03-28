/**
 * session.js — Oturum yönetimi (localStorage tabanlı)
 *
 * session.get()       → sicil no string veya null
 * session.set(sicil)  → kaydeder
 * session.setUser(u)  → { sicil, name, role } kaydeder
 * session.getUser()   → { sicil, name, role } veya null
 * session.getRole()   → "inspector" | "engineer" | "admin"
 * session.clear()     → siler
 */

const KEY      = 'qc_session_user';
const KEY_DATA = 'qc_session_data';

export const session = {
  get()          { return localStorage.getItem(KEY) || null; },
  set(sicil)     { localStorage.setItem(KEY, sicil.trim()); },
  clear()        { localStorage.removeItem(KEY); localStorage.removeItem(KEY_DATA); },

  setUser(data)  { localStorage.setItem(KEY_DATA, JSON.stringify(data)); },
  getUser()      {
    try { return JSON.parse(localStorage.getItem(KEY_DATA) || 'null'); }
    catch { return null; }
  },
  getRole()      { return this.getUser()?.role || 'inspector'; },
  getName()      { return this.getUser()?.name || this.get() || ''; },

  isAdmin()      { return this.getRole() === 'admin'; },
  isEngineer()   { return this.getRole() === 'engineer'; },
  isInspector()  { return this.getRole() === 'inspector'; },
  canWrite()     { return this.getRole() !== 'engineer'; },
};

/* =============================================
   BIBLIOTECA DIGITAL — Frontend App
   Alinhado ao Backend .NET + MongoDB + JWT
   ============================================= */

// ─── ESTADO GLOBAL ──────────────────────────────

const app             = document.getElementById('app');
const sessionStatus   = document.getElementById('session-status');
const navButtons      = document.querySelectorAll('[data-view]');
const toastContainer  = document.getElementById('toast-container');

const SESSION_KEY = 'biblioteca-session-v2';

let session     = loadSession();
let currentView = 'livros';


// ─── UTILITÁRIOS ────────────────────────────────

const esc = (v = '') =>
  String(v)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;');

const isLoggedIn = () => Boolean(session?.token);
const isAdmin    = () => session?.usuario?.Perfil === 'admin';

function loadSession() {
  try {
    return JSON.parse(localStorage.getItem(SESSION_KEY) || 'null');
  } catch {
    return null;
  }
}

function saveSession(value) {
  session = value;
  if (value) {
    localStorage.setItem(SESSION_KEY, JSON.stringify(value));
  } else {
    localStorage.removeItem(SESSION_KEY);
  }
  updateSessionStatus();
  updateAccountButton();
}

function updateSessionStatus() {
  if (!isLoggedIn()) {
    sessionStatus.textContent = 'Acesso público ao acervo';
    return;
  }
  const perfil = isAdmin() ? 'Administrador' : 'Usuário';
  sessionStatus.innerHTML = `
    <span style="display:inline-flex;align-items:center;gap:6px;">
      <span class="token-dot"></span>
      ${esc(session.usuario?.Nome)} · ${perfil}
    </span>`;
}

function updateAccountButton() {
  const btn = document.getElementById('conta-btn');
  if (!btn) return;
  btn.innerHTML = isLoggedIn()
    ? `<span class="nav-icon">👤</span> ${esc(session.usuario?.Nome?.split(' ')[0] || 'Conta')}`
    : `<span class="nav-icon">👤</span> Conta`;
}


// ─── HTTP CLIENT & NORMALIZADOR ──────────────────

const BASE_URL = 'http://localhost:5088/api/v1';

// Função para contornar a diferença de Case (camelCase vs PascalCase) entre .NET e JS
function normalizeData(obj) {
  if (Array.isArray(obj)) return obj.map(normalizeData);
  if (obj !== null && typeof obj === 'object') {
    const newObj = {};
    for (const key in obj) {
      const val = normalizeData(obj[key]);
      
      // Mantém a chave original intacta
      newObj[key] = val;
      
      // Cria versão PascalCase (ex: titulo -> Titulo)
      const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
      newObj[pascalKey] = val;
      
      // Cria versão camelCase (ex: Titulo -> titulo)
      const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
      newObj[camelKey] = val;
      
      // Mapeia o identificador _id ou id sempre para 'Id'
      if (key.toLowerCase() === '_id' || key.toLowerCase() === 'id') {
        newObj['Id'] = val;
      }
    }
    return newObj;
  }
  return obj;
}

async function request(path, options = {}) {
  const url = path.startsWith('http') ? path : `${BASE_URL}${path}`;
  
  const headers = { 'Content-Type': 'application/json' };
  if (session?.token) headers['Authorization'] = `Bearer ${session.token}`;
  
  if (options.headers) Object.assign(headers, options.headers);

  const res = await fetch(url, { ...options, headers });

  if (res.status === 204) return null;

  const contentType = res.headers.get('content-type') || '';
  let data = contentType.includes('application/json') ? await res.json() : null;

  // Aplica o fix para compatibilidade de propriedades
  data = normalizeData(data);

  if (!res.ok) {
    if (res.status === 401) {
      saveSession(null);
      toast('Sessão expirada. Faça login novamente.', 'error');
      setView('conta');
      throw new Error('Sessão expirada.');
    }
    if (res.status === 403) throw new Error('Acesso negado. Perfil insuficiente.');
    
    throw new Error(data?.mensagem || data?.message || 'Erro na requisição.');
  }

  return data;
}


// ─── TOAST NOTIFICATIONS ──────────────────────────

function toast(message, type = 'info') {
  const icons = { success: '✅', error: '❌', info: 'ℹ️', warning: '⚠️' };
  
  const el = document.createElement('div');
  el.className = `toast ${type === 'error' ? 'error' : type === 'success' ? 'success' : ''}`;
  el.innerHTML = `
    <span class="toast-icon">${icons[type] || icons.info}</span>
    <span>${esc(message)}</span>
    <button class="toast-close" aria-label="Fechar">×</button>
  `;
  
  el.querySelector('.toast-close').addEventListener('click', () => removeToast(el));
  toastContainer.appendChild(el);
  setTimeout(() => removeToast(el), 5000);
}

function removeToast(el) {
  if (!el.isConnected) return;
  el.classList.add('leaving');
  setTimeout(() => el.remove(), 300);
}


// ─── NAVEGAÇÃO ────────────────────────────────────

function setView(view) {
  currentView = view;
  
  navButtons.forEach(btn => btn.classList.toggle('active', btn.dataset.view === view));
  
  app.innerHTML = '';
  
  if (view === 'livros')  renderLivros();
  if (view === 'autores') renderAutores();
  if (view === 'conta')   renderConta();
}

navButtons.forEach(btn =>
  btn.addEventListener('click', () => setView(btn.dataset.view))
);


// ─── MODAL UTILITÁRIO ─────────────────────────────

function openModal(title, bodyHtml, { onOpen } = {}) {
  closeModal();
  
  const backdrop = document.createElement('div');
  backdrop.className = 'modal-backdrop';
  backdrop.id = 'modal-backdrop';
  
  backdrop.innerHTML = `
    <div class="modal" role="dialog" aria-modal="true" aria-label="${esc(title)}">
      <div class="modal-header">
        <h3>${esc(title)}</h3>
        <button class="modal-close" aria-label="Fechar">×</button>
      </div>
      <div class="modal-body">${bodyHtml}</div>
    </div>
  `;
  
  backdrop.querySelector('.modal-close').addEventListener('click', closeModal);
  backdrop.addEventListener('click', e => { if (e.target === backdrop) closeModal(); });
  
  document.body.appendChild(backdrop);
  
  if (onOpen) onOpen(backdrop.querySelector('.modal-body'));
}

function closeModal() {
  document.getElementById('modal-backdrop')?.remove();
}

function confirmDialog(message, onConfirm) {
  openModal('Confirmar ação', `
    <div class="confirm-dialog">
      <p>${esc(message)}</p>
      <div class="confirm-actions">
        <button class="btn btn-outline" id="confirm-cancel">Cancelar</button>
        <button class="btn btn-danger" id="confirm-ok">Confirmar</button>
      </div>
    </div>
  `, {
    onOpen(body) {
      body.querySelector('#confirm-cancel').addEventListener('click', closeModal);
      body.querySelector('#confirm-ok').addEventListener('click', () => {
        closeModal();
        onConfirm();
      });
    }
  });
}


// ─── VIEW: LIVROS ─────────────────────────────────

async function renderLivros() {
  app.innerHTML = `
    <div class="page-header">
      <h2>📖 Livros</h2>
      <p>Navegue pelo acervo da biblioteca${isAdmin() ? ' · como administrador, você pode gerenciar registros' : ''}</p>
    </div>

    <div class="toolbar">
      <form id="filter-form" class="filter-bar">
        <label>Título
          <input name="titulo" list="titulos-list" placeholder="Buscar título..." autocomplete="off" />
          <datalist id="titulos-list"></datalist>
        </label>
        <label>Categoria
          <input name="categoria" list="categorias-list" placeholder="Ficção, História..." autocomplete="off" />
          <datalist id="categorias-list"></datalist>
        </label>
        <label>Editora
          <input name="editora" list="editoras-list" placeholder="Editora..." autocomplete="off" />
          <datalist id="editoras-list"></datalist>
        </label>
        <button type="submit" class="btn btn-outline btn-sm">🔍 Filtrar</button>
        <button type="button" id="clear-filter" class="btn btn-ghost btn-sm">Limpar</button>
      </form>
      ${isAdmin() ? `<button class="btn btn-primary" id="btn-novo-livro">+ Novo Livro</button>` : ''}
    </div>

    <div id="livro-stats" class="stats-bar"></div>
    <div id="livro-list" class="cards-grid">
      <div class="loading"><div class="spinner"></div> Carregando acervo...</div>
    </div>
  `;

  document.getElementById('filter-form').addEventListener('submit', e => {
    e.preventDefault();
    const fd = new FormData(e.target);
    loadLivros(Object.fromEntries(fd));
  });

  document.getElementById('clear-filter')?.addEventListener('click', () => {
    document.getElementById('filter-form').reset();
    loadLivros();
  });

  document.getElementById('btn-novo-livro')?.addEventListener('click', () => openFormLivro());

  await loadLivros();
}

async function loadLivros(filtros = {}) {
  const list  = document.getElementById('livro-list');
  const stats = document.getElementById('livro-stats');
  if (!list) return;

  list.innerHTML = `<div class="loading"><div class="spinner"></div> Carregando...</div>`;

  try {
    const data = await request('/livros');
    let items = data?.dados ?? data?.items ?? [];

    const datalistTitulos = document.getElementById('titulos-list');
    const datalistCategorias = document.getElementById('categorias-list');
    const datalistEditoras = document.getElementById('editoras-list');
    
    // Alimenta dinamicamente os 3 datalists com os dados únicos do banco
    if (datalistTitulos && datalistCategorias && datalistEditoras) {
      const titulosUnicos = [...new Set(items.map(l => l.Titulo).filter(Boolean))].sort();
      const categoriasUnicas = [...new Set(items.map(l => l.Categoria).filter(Boolean))].sort();
      const editorasUnicas = [...new Set(items.map(l => l.Editora).filter(Boolean))].sort();

      datalistTitulos.innerHTML = titulosUnicos.map(t => `<option value="${esc(t)}">`).join('');
      datalistCategorias.innerHTML = categoriasUnicas.map(c => `<option value="${esc(c)}">`).join('');
      datalistEditoras.innerHTML = editorasUnicas.map(e => `<option value="${esc(e)}">`).join('');
    }

    if (filtros.titulo) {
      const q = filtros.titulo.toLowerCase();
      items = items.filter(l => l.Titulo?.toLowerCase().includes(q));
    }
    if (filtros.categoria) {
      const q = filtros.categoria.toLowerCase();
      items = items.filter(l => l.Categoria?.toLowerCase().includes(q));
    }
    if (filtros.editora) {
      const q = filtros.editora.toLowerCase();
      items = items.filter(l => l.Editora?.toLowerCase().includes(q));
    }

    if (stats) {
      stats.innerHTML = `<strong>${items.length}</strong> livro${items.length !== 1 ? 's' : ''} encontrado${items.length !== 1 ? 's' : ''}`;
    }

    if (!items.length) {
      list.innerHTML = `
        <div class="empty-state">
          <span class="empty-icon">📭</span>
          <p>Nenhum livro encontrado no acervo.</p>
        </div>`;
      return;
    }

    list.innerHTML = items.map(livro => `
      <div class="card" data-id="${livro.Id}">
        <div class="card-header">
          <div>
            <div class="card-title">${esc(livro.Titulo)}</div>
            <div class="card-subtitle">${esc(livro.Autor?.Nome || `Autor ID: ${livro.AutorId}`)}</div>
          </div>
          ${livro.Categoria ? `<span class="badge badge-category">${esc(livro.Categoria)}</span>` : ''}
        </div>
        <div class="card-body">
          <div style="display:flex;flex-direction:column;gap:4px;margin-top:6px;">
            ${livro.ISBN        ? `<span>📌 ISBN: ${esc(livro.ISBN)}</span>` : ''}
            ${livro.AnoPublicacao ? `<span>📅 ${livro.AnoPublicacao}</span>` : ''}
            ${livro.Editora     ? `<span>🏛️ ${esc(livro.Editora)}</span>` : ''}
            ${livro.Sinopse     ? `<span style="margin-top:6px;font-style:italic;opacity:.8">${esc(livro.Sinopse.substring(0, 120))}${livro.Sinopse.length > 120 ? '…' : ''}</span>` : ''}
          </div>
        </div>
        ${isAdmin() ? `
        <div class="card-footer">
          <button class="btn btn-ghost btn-sm btn-edit-livro" data-id="${livro.Id}">✏️ Editar</button>
          <button class="btn btn-danger btn-sm btn-del-livro" data-id="${livro.Id}">🗑️ Excluir</button>
        </div>` : ''}
      </div>
    `).join('');

    list.querySelectorAll('.btn-del-livro').forEach(btn => {
      btn.addEventListener('click', () => {
        const id = btn.dataset.id;
        const titulo = btn.closest('.card').querySelector('.card-title').textContent;
        confirmDialog(`Excluir o livro "${titulo}"?`, async () => {
          try {
            await request(`/livros/${id}`, { method: 'DELETE' });
            toast('Livro excluído com sucesso.', 'success');
            loadLivros();
          } catch (err) {
            toast(err.message, 'error');
          }
        });
      });
    });

    list.querySelectorAll('.btn-edit-livro').forEach(btn => {
      btn.addEventListener('click', async () => {
        const id = btn.dataset.id;
        try {
          const livro = await request(`/livros/${id}`);
          openFormLivro(livro); 
        } catch (err) {
          toast(err.message, 'error');
        }
      });
    });

  } catch (err) {
    list.innerHTML = `<div class="empty-state"><span class="empty-icon">⚠️</span><p>${esc(err.message)}</p></div>`;
  }
}

async function openFormLivro(livro = null) {
  let autores = [];
  try {
    const res = await request('/autores');
    autores = res?.dados ?? res?.items ?? [];
  } catch {
    toast('Erro ao carregar autores.', 'error');
  }

  const isEdit = Boolean(livro);
  const title  = isEdit ? 'Editar Livro' : 'Cadastrar Novo Livro';

  const autorOptions = autores.map(a =>
    `<option value="${a.Id}" ${livro?.AutorId === a.Id ? 'selected' : ''}>${esc(a.Nome)}</option>`
  ).join('');

  openModal(title, `
    <form id="form-livro" class="form-grid">
      <label class="full-width">Título *
        <input name="Titulo" required value="${esc(livro?.Titulo || '')}" placeholder="Título do livro" />
      </label>
      <label>Autor *
        <select name="AutorId" required>
          <option value="">Selecione...</option>
          ${autorOptions}
        </select>
      </label>
      <label>ISBN
        <input name="ISBN" value="${esc(livro?.ISBN || '')}" placeholder="978-0-000-00000-0" />
      </label>
      <label>Ano de Publicação
        <input name="AnoPublicacao" type="number" value="${livro?.AnoPublicacao || ''}" placeholder="2024" min="1000" max="2099" />
      </label>
      <label>Editora
        <input name="Editora" value="${esc(livro?.Editora || '')}" placeholder="Nome da editora" />
      </label>
      <label>Categoria
        <input name="Categoria" value="${esc(livro?.Categoria || '')}" placeholder="Ficção, Romance..." />
      </label>
      <label class="full-width">Sinopse
        <textarea name="Sinopse" placeholder="Resumo do livro...">${esc(livro?.Sinopse || '')}</textarea>
      </label>
      <div class="full-width form-actions">
        <button type="button" class="btn btn-outline" id="cancel-form">Cancelar</button>
        <button type="submit" class="btn btn-primary">${isEdit ? '💾 Salvar' : '+ Cadastrar'}</button>
      </div>
    </form>
  `, {
    onOpen(body) {
      body.querySelector('#cancel-form').addEventListener('click', closeModal);
      
      body.querySelector('#form-livro').addEventListener('submit', async e => {
        e.preventDefault();
        
        const fd = new FormData(e.target);
        const dto = Object.fromEntries(fd);
        
        if (dto.AnoPublicacao) dto.AnoPublicacao = Number(dto.AnoPublicacao);
        
        ['ISBN', 'Editora', 'Categoria', 'Sinopse'].forEach(k => { if (!dto[k]) delete dto[k]; });

        const submitBtn = e.target.querySelector('[type=submit]');
        submitBtn.disabled = true;
        submitBtn.textContent = 'Salvando...';

        try {
          if (isEdit) {
            await request(`/livros/${livro.Id}`, { method: 'PUT', body: JSON.stringify(dto) });
            toast('Livro atualizado com sucesso!', 'success');
          } else {
            await request('/livros', { method: 'POST', body: JSON.stringify(dto) });
            toast('Livro cadastrado com sucesso!', 'success');
          }
          closeModal();
          loadLivros();
        } catch (err) {
          toast(err.message, 'error');
          submitBtn.disabled = false;
          submitBtn.textContent = isEdit ? '💾 Salvar' : '+ Cadastrar';
        }
      });
    }
  });
}


// ─── VIEW: AUTORES ────────────────────────────────

async function renderAutores() {
  app.innerHTML = `
    <div class="page-header">
      <h2>✍️ Autores</h2>
      <p>Autores cadastrados no sistema${isAdmin() ? ' · como administrador, você pode gerenciar registros' : ''}</p>
    </div>

    <div class="toolbar">
      <div></div>
      ${isAdmin() ? `<button class="btn btn-primary" id="btn-novo-autor">+ Novo Autor</button>` : ''}
    </div>

    <div id="autor-stats" class="stats-bar"></div>
    <div id="autor-list" class="cards-grid">
      <div class="loading"><div class="spinner"></div> Carregando autores...</div>
    </div>
  `;

  document.getElementById('btn-novo-autor')?.addEventListener('click', () => openFormAutor());

  await loadAutores();
}

async function loadAutores() {
  const list  = document.getElementById('autor-list');
  const stats = document.getElementById('autor-stats');
  if (!list) return;

  try {
    const data = await request('/autores');
    const items = data?.dados ?? data?.items ?? [];

    if (stats) {
      stats.innerHTML = `<strong>${items.length}</strong> autor${items.length !== 1 ? 'es' : ''} cadastrado${items.length !== 1 ? 's' : ''}`;
    }

    if (!items.length) {
      list.innerHTML = `
        <div class="empty-state">
          <span class="empty-icon">✍️</span>
          <p>Nenhum autor cadastrado ainda.</p>
        </div>`;
      return;
    }

    list.innerHTML = items.map(autor => {
      const nascimento = autor.DataNascimento
        ? new Date(autor.DataNascimento).getFullYear()
        : null;
        
      return `
        <div class="card" data-id="${autor.Id}">
          <div class="card-header">
            <div>
              <div class="card-title">${esc(autor.Nome)}</div>
              <div class="card-subtitle">
                ${autor.Nacionalidade ? `🌍 ${esc(autor.Nacionalidade)}` : ''}
                ${nascimento ? ` · n. ${nascimento}` : ''}
              </div>
            </div>
          </div>
          <div class="card-body">
            ${autor.Biografia
              ? `<p style="font-style:italic;margin-top:6px;opacity:.8">${esc(autor.Biografia.substring(0, 150))}${autor.Biografia.length > 150 ? '…' : ''}</p>`
              : ''}
          </div>
          <div class="card-footer" style="justify-content:space-between;align-items:center">
            <button class="btn btn-ghost btn-sm btn-livros-autor" data-id="${autor.Id}" data-nome="${esc(autor.Nome)}">
              📚 Ver livros
            </button>
            ${isAdmin() ? `
            <div style="display:flex;gap:8px">
              <button class="btn btn-ghost btn-sm btn-edit-autor" data-id="${autor.Id}">✏️ Editar</button>
              <button class="btn btn-danger btn-sm btn-del-autor" data-id="${autor.Id}">🗑️ Excluir</button>
            </div>` : ''}
          </div>
        </div>
      `;
    }).join('');

    list.querySelectorAll('.btn-del-autor').forEach(btn => {
      btn.addEventListener('click', () => {
        const id   = btn.dataset.id;
        const nome = btn.closest('.card').querySelector('.card-title').textContent;
        confirmDialog(`Excluir o autor "${nome}"?`, async () => {
          try {
            await request(`/autores/${id}`, { method: 'DELETE' });
            toast('Autor excluído com sucesso.', 'success');
            loadAutores();
          } catch (err) {
            toast(err.message, 'error');
          }
        });
      });
    });

    list.querySelectorAll('.btn-edit-autor').forEach(btn => {
      btn.addEventListener('click', async () => {
        try {
          const autor = await request(`/autores/${btn.dataset.id}`);
          openFormAutor(autor);
        } catch (err) {
          toast(err.message, 'error');
        }
      });
    });

    list.querySelectorAll('.btn-livros-autor').forEach(btn => {
      btn.addEventListener('click', async () => {
        const id   = btn.dataset.id;
        const nome = btn.dataset.nome;
        try {
          const res = await request(`/autores/${id}/com-livros`);
          const livros = res?.livros ?? [];
          
          const livrosHtml = livros.length
            ? livros.map(l => `<li style="padding:6px 0;border-bottom:1px solid var(--border)">${esc(l.Titulo)} ${l.AnoPublicacao ? `(${l.AnoPublicacao})` : ''}</li>`).join('')
            : `<li style="color:var(--ink-light);font-style:italic">Nenhum livro cadastrado para este autor.</li>`;
          
          openModal(`📚 Livros de ${nome}`, `
            <p style="margin-bottom:16px;color:var(--ink-muted);font-size:13px">${livros.length} livro(s) no acervo</p>
            <ul style="list-style:none;padding:0;margin:0">${livrosHtml}</ul>
          `);
        } catch (err) {
          toast(err.message, 'error');
        }
      });
    });

  } catch (err) {
    list.innerHTML = `<div class="empty-state"><span class="empty-icon">⚠️</span><p>${esc(err.message)}</p></div>`;
  }
}

function openFormAutor(autor = null) {
  const isEdit = Boolean(autor);

  let nascimentoValue = '';
  if (autor?.DataNascimento) {
    nascimentoValue = autor.DataNascimento.split('T')[0];
  }

  openModal(isEdit ? 'Editar Autor' : 'Cadastrar Novo Autor', `
    <form id="form-autor" class="form-grid">
      <label class="full-width">Nome *
        <input name="Nome" required value="${esc(autor?.Nome || '')}" placeholder="Nome completo do autor" />
      </label>
      <label>Data de Nascimento
        <input name="DataNascimento" type="date" value="${nascimentoValue}" />
      </label>
      <label>Nacionalidade
        <input name="Nacionalidade" value="${esc(autor?.Nacionalidade || '')}" placeholder="Ex: Brasileira" />
      </label>
      <label class="full-width">Biografia
        <textarea name="Biografia" placeholder="Breve biografia do autor...">${esc(autor?.Biografia || '')}</textarea>
      </label>
      <div class="full-width form-actions">
        <button type="button" class="btn btn-outline" id="cancel-autor">Cancelar</button>
        <button type="submit" class="btn btn-primary">${isEdit ? '💾 Salvar' : '+ Cadastrar'}</button>
      </div>
    </form>
  `, {
    onOpen(body) {
      body.querySelector('#cancel-autor').addEventListener('click', closeModal);
      
      body.querySelector('#form-autor').addEventListener('submit', async e => {
        e.preventDefault();
        const dto = Object.fromEntries(new FormData(e.target));
        
        if (!dto.DataNascimento) delete dto.DataNascimento;
        if (!dto.Nacionalidade) delete dto.Nacionalidade;
        if (!dto.Biografia) delete dto.Biografia;

        const submitBtn = e.target.querySelector('[type=submit]');
        submitBtn.disabled = true;
        submitBtn.textContent = 'Salvando...';

        try {
          if (isEdit) {
            await request(`/autores/${autor.Id}`, { method: 'PUT', body: JSON.stringify(dto) });
            toast('Autor atualizado com sucesso!', 'success');
          } else {
            await request('/autores', { method: 'POST', body: JSON.stringify(dto) });
            toast('Autor cadastrado com sucesso!', 'success');
          }
          closeModal();
          loadAutores();
        } catch (err) {
          toast(err.message, 'error');
          submitBtn.disabled = false;
          submitBtn.textContent = isEdit ? '💾 Salvar' : '+ Cadastrar';
        }
      });
    }
  });
}


// ─── VIEW: CONTA ──────────────────────────────────

function renderConta() {
  app.innerHTML = `
    <div class="page-header">
      <h2>👤 Conta</h2>
      <p>${isLoggedIn() ? 'Sua sessão ativa na Biblioteca Digital' : 'Faça login ou registre-se para acessar recursos adicionais'}</p>
    </div>
    <div id="conta-content"></div>
  `;

  const content = document.getElementById('conta-content');
  
  content.innerHTML = isLoggedIn() ? renderPerfil() : renderAuthForms();

  if (isLoggedIn()) {
    document.getElementById('btn-logout')?.addEventListener('click', () => {
      confirmDialog('Deseja encerrar sua sessão?', () => {
        saveSession(null);
        toast('Sessão encerrada.', 'info');
        setView('livros');
      });
    });

    if (isAdmin()) {
      document.getElementById('btn-novo-admin')?.addEventListener('click', openFormAdmin);
    }
  } else {
    document.getElementById('form-login')?.addEventListener('submit', handleLogin);
    document.getElementById('form-registro')?.addEventListener('submit', handleRegistro);
  }
}

function renderPerfil() {
  const u = session.usuario;
  const inicial = (u?.Nome || '?').charAt(0).toUpperCase();
  const perfil  = u?.Perfil === 'admin' ? 'admin' : 'usuario';
  const expiraEm = session.expiraEm
    ? new Date(session.expiraEm).toLocaleString('pt-BR')
    : 'Indisponível';

  return `
    <div class="profile-panel">
      <div class="profile-avatar">${esc(inicial)}</div>
      <div class="profile-name">${esc(u?.Nome || 'Usuário')}</div>
      <div class="profile-email">${esc(u?.Email || '')}</div>
      <div class="profile-meta">
        <div class="profile-row">
          <span>Perfil</span>
          <span class="badge ${perfil === 'admin' ? 'badge-admin' : 'badge-user'}">${esc(u?.Perfil || perfil)}</span>
        </div>
        <div class="profile-row">
          <span>Status</span>
          <span class="badge badge-active">● Ativo</span>
        </div>
        <div class="profile-row">
          <span>Token expira em</span>
          <span style="font-size:13px;color:var(--ink-muted)">${esc(expiraEm)}</span>
        </div>
        <div class="profile-row">
          <span>ID MongoDB</span>
          <span style="font-size:12px;font-family:monospace;color:var(--ink-muted)">${esc(String(u?.Id || ''))}</span>
        </div>
      </div>
      <div class="profile-actions">
        <button class="btn btn-outline btn-full" id="btn-logout">🚪 Encerrar sessão</button>
      </div>
      ${isAdmin() ? `
        <div style="margin-top:16px">
          <div class="info-block">
            🔑 Você está logado como <strong>administrador</strong>. Pode cadastrar e excluir livros e autores.
          </div>
          <button class="btn btn-primary btn-full" id="btn-novo-admin" style="margin-top: 10px;">
            ➕ Cadastrar Novo Administrador
          </button>
        </div>
      ` : ''}
    </div>
  `;
}

function openFormAdmin() {
  openModal('Cadastrar Novo Administrador', `
    <form id="form-registro-admin" class="form-grid">
      <label class="full-width">Nome *
        <input name="Nome" required placeholder="Nome do novo administrador" />
      </label>
      <label class="full-width">E-mail *
        <input name="Email" type="email" required placeholder="admin@email.com" />
      </label>
      <label class="full-width">Senha *
        <input name="Senha" type="password" required placeholder="Mínimo 6 caracteres" minlength="6" />
      </label>
      <div class="full-width form-actions">
        <button type="button" class="btn btn-outline" id="cancel-admin">Cancelar</button>
        <button type="submit" class="btn btn-primary" id="btn-submit-admin">Criar Conta</button>
      </div>
    </form>
  `, {
    onOpen(body) {
      body.querySelector('#cancel-admin').addEventListener('click', closeModal);
      
      body.querySelector('#form-registro-admin').addEventListener('submit', async e => {
        e.preventDefault();
        const btn = document.getElementById('btn-submit-admin');
        btn.disabled = true;
        btn.textContent = 'Criando...';

        try {
          const dto = Object.fromEntries(new FormData(e.target));
          
          await request('/auth/registro-admin', {
            method: 'POST',
            body: JSON.stringify(dto)
          });
          
          toast('Novo administrador cadastrado com sucesso!', 'success');
          closeModal();
        } catch (err) {
          toast(err.message, 'error');
          btn.disabled = false;
          btn.textContent = 'Criar Conta';
        }
      });
    }
  });
}

function renderAuthForms() {
  return `
    <div class="auth-container">
      <div class="auth-grid">
        <div class="auth-card">
          <h3>Entrar</h3>
          <p class="auth-subtitle">Acesse sua conta com e-mail e senha</p>
          <form id="form-login" class="form-grid">
            <label>E-mail *
              <input name="Email" type="email" required placeholder="seu@email.com" autocomplete="email" />
            </label>
            <label>Senha *
              <input name="Senha" type="password" required placeholder="••••••••" autocomplete="current-password" />
            </label>
            <div class="form-actions" style="margin-top:8px">
              <button type="submit" class="btn btn-primary btn-full" id="btn-login">Entrar →</button>
            </div>
          </form>
        </div>

        <div class="auth-card">
          <h3>Registrar</h3>
          <p class="auth-subtitle">Crie uma conta de usuário para acessar o acervo</p>
          <form id="form-registro" class="form-grid">
            <label>Nome *
              <input name="Nome" required placeholder="Seu nome completo" autocomplete="name" />
            </label>
            <label>E-mail *
              <input name="Email" type="email" required placeholder="seu@email.com" autocomplete="email" />
            </label>
            <label>Senha *
              <input name="Senha" type="password" required placeholder="Mínimo 6 caracteres" autocomplete="new-password" minlength="6" />
            </label>
            <div class="form-actions" style="margin-top:8px">
              <button type="submit" class="btn btn-primary btn-full" id="btn-registro">Criar conta</button>
            </div>
          </form>
          <p style="font-size:12px;color:var(--ink-light);margin-top:12px;text-align:center">
            Contas públicas são criadas com perfil <strong>usuário</strong>.<br/>
            Administradores são criados por outro admin.
          </p>
        </div>
      </div>
    </div>
  `;
}

async function handleLogin(e) {
  e.preventDefault();
  const btn = document.getElementById('btn-login');
  btn.disabled = true;
  btn.textContent = 'Entrando...';

  try {
    const body = Object.fromEntries(new FormData(e.target));
    const result = await request('/auth/login', {
      method: 'POST',
      body: JSON.stringify(body)
    });
    
    saveSession({
      token:    result.token,
      expiraEm: result.expiraEm,
      usuario:  result.usuario
    });
    
    toast(`Bem-vindo, ${result.usuario?.Nome || 'usuário'}! 👋`, 'success');
    setView('livros');
  } catch (err) {
    toast(err.message, 'error');
    btn.disabled = false;
    btn.textContent = 'Entrar →';
  }
}

async function handleRegistro(e) {
  e.preventDefault();
  const btn = document.getElementById('btn-registro');
  btn.disabled = true;
  btn.textContent = 'Criando conta...';

  try {
    const body = Object.fromEntries(new FormData(e.target));
    const result = await request('/auth/registro', {
      method: 'POST',
      body: JSON.stringify(body)
    });
    
    saveSession({
      token:    result.token,
      expiraEm: result.expiraEm,
      usuario:  result.usuario
    });
    
    toast('Conta criada e sessão iniciada! 🎉', 'success');
    setView('livros');
  } catch (err) {
    toast(err.message, 'error');
    btn.disabled = false;
    btn.textContent = 'Criar conta';
  }
}


// ─── INICIALIZAÇÃO ────────────────────────────────

updateSessionStatus();
updateAccountButton();
setView('livros');
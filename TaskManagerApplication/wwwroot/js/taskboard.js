const api = '/api/tasks';

const LOCAL_KEY = 'taskboard_tasks_v1';
const state = { tasks: [] };

async function loadFromServer() {
  const res = await fetch(api);
  if (!res.ok) throw new Error('Server error');
  const tasks = await res.json();
  state.tasks = tasks;
  saveLocal();
  render();
}

function loadLocal() {
  try {
    const raw = localStorage.getItem(LOCAL_KEY);
    if (!raw) return false;
    state.tasks = JSON.parse(raw);
    return true;
  } catch (e) { console.error('Failed to load local tasks', e); return false; }
}

function saveLocal() {
  try { localStorage.setItem(LOCAL_KEY, JSON.stringify(state.tasks)); } catch (e) { console.error('Failed to save tasks', e); }
}

async function load() {
  // Try server first, fall back to local storage
  try {
    await loadFromServer();
  } catch (e) {
    console.warn('Server load failed, falling back to localStorage', e);
    if (loadLocal()) render();
  }
}

function formatDate(d) { if (!d) return ''; return new Date(d).toLocaleDateString(); }
function formatDateTime(d) { if (!d) return ''; return new Date(d).toLocaleString(); }

function render() {
  const search = document.getElementById('search').value.toLowerCase();
  const priority = document.getElementById('priorityFilter').value;
  ['todo','inprogress','done'].forEach(status => {
    const list = document.querySelector(`#${status}List`);
    list.innerHTML = '';
    state.tasks
      .filter(t => t.status === status)
      .filter(t => (!search || (t.title+" "+(t.description||'')).toLowerCase().includes(search)))
      .filter(t => (!priority || t.priority === priority))
      .forEach(addTaskCard);
  });
}

function addTaskCard(task) {
  const list = document.querySelector(`#${task.status}List`);
  const card = document.createElement('div');
  card.className = 'card';
  card.draggable = true;
  card.dataset.id = task.id;
  if (task.dueDate && new Date(task.dueDate) < new Date()) card.classList.add('overdue');

  card.innerHTML = `<div class="card-head"><strong>${escapeHtml(task.title)}</strong>
    <button class="del">×</button></div>
    <div class="meta">${escapeHtml(task.priority)} • ${formatDate(task.dueDate)} <span class="entered">(Entered: ${formatDateTime(task.enteredOn)})</span></div>
    <div class="desc">${escapeHtml(task.description||'')}</div>`;

  card.querySelector('.del').addEventListener('click', async e => {
    e.stopPropagation();
    try {
      const res = await fetch(`${api}/${task.id}`, { method: 'DELETE' });
      if (!res.ok) throw new Error('Delete failed');
      await load();
    } catch (err) {
      // fallback: remove locally
      state.tasks = state.tasks.filter(t => t.id !== task.id);
      saveLocal();
      render();
    }
  });

  card.addEventListener('dblclick', async () => {
    const title = prompt('Edit title', task.title) || task.title;
    const desc = prompt('Edit description', task.description) ?? task.description;
    task.title = title; task.description = desc;
    try {
      const res = await fetch(`${api}/${task.id}`, { method: 'PUT', headers:{'Content-Type':'application/json'}, body: JSON.stringify(task) });
      if (!res.ok) throw new Error('Update failed');
      await load();
    } catch (err) {
      // fallback: update local
      const idx = state.tasks.findIndex(t => t.id === task.id);
      if (idx >= 0) { state.tasks[idx] = task; saveLocal(); render(); }
    }
  });

  card.addEventListener('dragstart', e => { e.dataTransfer.setData('text/plain', task.id); });

  const listEl = document.querySelector(`#${task.status}List`);
  listEl.appendChild(card);
}

function escapeHtml(s){ return String(s).replace(/[&<>"']/g, c=>({"&":"&amp;","<":"&lt;",">":"&gt;","\"":"&quot;","'":"&#39;"}[c])); }

function setupDragDrop(){
  document.querySelectorAll('.column').forEach(col=>{
    col.addEventListener('dragover', e=>{ e.preventDefault(); col.classList.add('over'); });
    col.addEventListener('dragleave', e=>{ col.classList.remove('over'); });
    col.addEventListener('drop', async e=>{
      e.preventDefault(); col.classList.remove('over');
      const id = e.dataTransfer.getData('text/plain');
      const task = state.tasks.find(t=>t.id==id);
      if (!task) return;
      const status = col.dataset.status;
      task.status = status;
      try {
        const res = await fetch(`${api}/${task.id}`, { method: 'PUT', headers:{'Content-Type':'application/json'}, body: JSON.stringify(task) });
        if (!res.ok) throw new Error('Update failed');
        await load();
      } catch (err) {
        // fallback local update
        const idx = state.tasks.findIndex(t => t.id === task.id);
        if (idx >= 0) { state.tasks[idx] = task; saveLocal(); render(); }
      }
    });
  });
}

document.addEventListener('DOMContentLoaded', ()=>{
  document.getElementById('createBtn').addEventListener('click', async ()=>{
    const title = document.getElementById('titleInput').value.trim();
    if (!title) return alert('Title required');
    const due = document.getElementById('dueInput').value || null;
    const priority = document.getElementById('priorityInput').value;
    const dto = { title, description: '', priority, dueDate: due, status: 'todo', enteredOn: new Date().toISOString() };
    try {
      const res = await fetch(api, { method: 'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify(dto) });
      if (!res.ok) throw new Error('Create failed');
      await load();
    } catch (err) {
      // fallback: assign temp id and save locally
      dto.id = Date.now();
      state.tasks.push(dto);
      saveLocal();
      render();
    }
    document.getElementById('titleInput').value = '';
  });

  document.getElementById('search').addEventListener('input', render);
  document.getElementById('priorityFilter').addEventListener('change', render);

  const darkToggle = document.getElementById('darkToggle');
  const darkPref = localStorage.getItem('dark') === '1';
  darkToggle.checked = darkPref;
  document.body.classList.toggle('dark', darkPref);
  darkToggle.addEventListener('change', ()=>{ const d = darkToggle.checked; document.body.classList.toggle('dark', d); localStorage.setItem('dark', d? '1':'0'); });

  setupDragDrop();
  // Load local first for instant UI, then refresh from server
  if (!loadLocal()) render();
  load();
});

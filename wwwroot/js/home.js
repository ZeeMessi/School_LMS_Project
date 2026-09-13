const hamburgerBtn = document.getElementById('hamburgerBtn');
const sidebar = document.getElementById('sidebar');
const backdrop = document.getElementById('sidebarBackdrop');


// Sidebar starts open when Home loads.
document.body.classList.add('sidebar-open');


// Open / close sidebar
hamburgerBtn.addEventListener('click', () => {

    sidebar.classList.toggle('open');
    backdrop.classList.toggle('open');
    document.body.classList.toggle('sidebar-open');

});


// Clicking outside the sidebar closes it.
backdrop.addEventListener('click', () => {

    sidebar.classList.remove('open');
    backdrop.classList.remove('open');
    document.body.classList.remove('sidebar-open');

});
// Toggles the password field between hidden (dots) and visible (plain text),
// and swaps the eye icon so the user has a clear visual cue of the current state.
const toggleBtn = document.getElementById('togglePassword');
const passwordInput = document.getElementById('Password');
const eyeOpen = document.getElementById('eyeOpen');
const eyeClosed = document.getElementById('eyeClosed');

toggleBtn.addEventListener('click', () => {
    const isHidden = passwordInput.type === 'password';
    passwordInput.type = isHidden ? 'text' : 'password';

    eyeOpen.style.display = isHidden ? 'none' : 'inline';
    eyeClosed.style.display = isHidden ? 'inline' : 'none';

    toggleBtn.setAttribute('aria-label', isHidden ? 'Hide password' : 'Show password');
});
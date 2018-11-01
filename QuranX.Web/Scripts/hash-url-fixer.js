var shiftWindow = function () { scrollBy(0, -50); };
window.addEventListener("hashchange", shiftWindow);
$(document).ready(function () {
	if (window.location.hash) shiftWindow();
});

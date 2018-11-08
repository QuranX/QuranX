var shiftWindow = function (anchorName) {
	if (anchorName.charAt(0) == '#') {
		anchorName = anchorName.substring(1);
	}
	var tag = $('a[name="' + anchorName + '"]');
	if (tag.length) {
		$('html,body').animate({ scrollTop: tag.offset().top - 75 }, 'slow');
	}
};
$(document).ready(function () {
	shiftWindow(window.location.hash || "focal-point");
});

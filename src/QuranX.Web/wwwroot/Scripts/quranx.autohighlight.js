function getParameterByName(name) {
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
		results = regex.exec(location.search);
	return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

console.log('a');
$(function () {
	console.log('b');
	var hl = getParameterByName("hl");
	if (hl != null) {
		$(".highlightable").highlight(hl.split(","));
	}
});

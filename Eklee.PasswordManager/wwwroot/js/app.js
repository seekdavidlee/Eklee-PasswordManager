window.focusElement = (id) => {
	setTimeout(() => {
		var e = document.getElementById(id);
		if (e) {
			e.focus();
		}
	}, 100);
}
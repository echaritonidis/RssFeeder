function saveFile(file, content) {
    var link = document.createElement('a');
    link.download = name;
    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + content
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
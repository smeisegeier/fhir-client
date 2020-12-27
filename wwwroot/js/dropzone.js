// see https://www.dropzonejs.com/#configuration
$(document).ready(function () {
    Dropzone.options.uploader =
    {
        paramName: "file",
        uploadMultiple = "false",
        maxFilesize: 5,
        accept: function (file, done) {
            // this is just joking
            if (file.name == "test.jpg") {
                alert("Can't upload a test file.");
            }
            else {
                done();
            }
        }
    };
});
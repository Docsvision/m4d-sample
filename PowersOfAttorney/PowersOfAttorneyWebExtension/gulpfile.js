const gulp = require("gulp");
const docsvision = require("@docsvision/webclient-extension-build/gulpfile.js");
const { STYLES_DIR, PLUGINS, PLUGINS_DIR } = require("./copy-path");
const log = require('fancy-log');
const colors = require('ansi-colors');
const path = require('path');

var sources = { };
sources.src = "src";
sources.scss = sources.src + "/**/*.scss";

gulp.task("copy-plugins", () => {
    return gulp.src(PLUGINS)
    .pipe(gulp.dest(PLUGINS_DIR))
    .on('end', () => log(colors.green("Plugins copied in " + path.resolve(PLUGINS_DIR))))
})

gulp.task("clean-styles", function() {
    return docsvision.cleanStyles(STYLES_DIR);
});

gulp.task("build-styles", function() {
    return docsvision.buildStyles(STYLES_DIR, sources.scss);
});

gulp.task("styles", gulp.series("clean-styles", "build-styles"));

gulp.task('watch', function(){
    gulp.watch([sources.scss], ['build-styles']);
});
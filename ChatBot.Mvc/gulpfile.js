"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    // cssmin = require("gulp-cssmin"),
    cleanCSS = require('gulp-clean-css'),
    jsmin = require("gulp-terser"),
    //uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json");

var regex = {
    css: /\.css$/,
    js: /\.js$/
};

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}

function clean() {
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });

    return del(files);
}
gulp.task(clean);
function minJs() {
    var tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: ".", allowEmpty: true })
            .pipe(concat(bundle.outputFileName))
            //.pipe(uglify())
            .pipe(jsmin())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
}
gulp.task(minJs);

function minCss() {
    var tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(cleanCSS({ debug: true }, (details) => {
                console.log('############# FILE ' + bundle.outputFileName)
                //console.dir(details);
                console.log('Original: ' + details.stats.originalSize.toLocaleString('pt-BR') + ' kb - Minificado: ' + details.stats.minifiedSize.toLocaleString('pt-BR') + ' kb');
                console.log('efficiency ' + (details.stats.efficiency * 100).toFixed(1) + '% - em ' + details.stats.timeSpent + ' ms');
            }))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
}
gulp.task(minCss);

gulp.task("minAll", gulp.series(clean, minJs, minCss));
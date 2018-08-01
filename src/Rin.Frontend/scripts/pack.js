const paths = require('../config/paths');

const fs = require('fs');
const archiver = require('archiver');
const output = fs.createWriteStream(paths.rinFrontendResourcesProject + "/Resources.zip");
const archive = archiver('zip', {});

archive.pipe(output);
archive.directory(paths.appBuild, false);
archive.finalize();
const outputPath = '../Rin/Resources.zip';
const distDirPath = './dist/';

const fs = require('fs');
const archiver = require('archiver');
const output = fs.createWriteStream(outputPath);
const archive = archiver('zip', {});

archive.pipe(output);
archive.directory(distDirPath, false);
archive.finalize();
const TerserWebpackPlugin = require("terser-webpack-plugin");
const webpackMerge = require('webpack-merge');
const configBase = require('./webpack.config.js');

module.exports = webpackMerge.merge(configBase, {
    mode: 'production',
    devtool: false,
    
    optimization: {
        minimizer: [
            new TerserWebpackPlugin()
        ]
    },
});
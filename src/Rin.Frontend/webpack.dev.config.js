const webpackMerge = require('webpack-merge');
const configBase = require('./webpack.config.js');

module.exports = webpackMerge.merge(configBase, {
    output: {
        publicPath: '/',
    },
    mode: 'development',
    devtool: 'inline-source-map',
    devServer: {
        historyApiFallback: true
    }
});
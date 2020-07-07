const path = require("path");
const MonacoEditorWebpackPlugin = require("monaco-editor-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
    devtool: false,
    entry: {
        main: path.join(__dirname, "src"),
    },
    output: {
        filename: "static/js/[name].[chunkhash:8].js",
        chunkFilename: "static/js/[name].[chunkhash:8].chunk.js"
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                include:[/node_modules/],
                use: [
                    { loader: "style-loader" },
                    { loader: "css-loader" },
                ]
            },
            {
                test: /\.css$/,
                exclude:[/node_modules/],
                use: [
                    { loader: "style-loader" },
                    {
                        loader: "css-loader",
                        options: {
                            importLoaders: 1,
                            modules: true,
                        },
                    },
                ]
            },
            {
                test: /\.ttf$/,
                use: [
                    { loader: 'file-loader' },
                ]
            },
            {
                test: /\.tsx?$/,
                use: [
                    {
                        loader: "ts-loader",
                        options: {
                            transpileOnly: true
                        }
                    },
                ],
            },
        ],
    },
    resolve: {
        extensions: [".js", ".ts", ".tsx"],
    },
    plugins: [
        new MonacoEditorWebpackPlugin({
            filename: 'static/js/monaco/[name].worker.js',
        }),
        new HtmlWebpackPlugin({
            template: "src/index.html"
        }),
        new CopyWebpackPlugin({
            patterns: [
                { from: 'public', to: './' },
            ],
        }),
    ],
};
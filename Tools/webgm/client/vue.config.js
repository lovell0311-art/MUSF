const JavaScriptObfuscator = require('webpack-obfuscator');
const CompressionPlugin = require("compression-webpack-plugin");

const timeStamp = new Date().getTime();

module.exports = {
    productionSourceMap: false,
    devServer: {
        // 下面为需要跨域的
        proxy: {//配置跨域
            '/api': {
                target: 'http://localhost:5000/api/',//这里后台的地址模拟的;应该填写你们真实的后台接口
                ws: true,
                changOrigin: true,//允许跨域
                pathRewrite: {
                    '^/api': ''//请求的时候使用这个api就可以
                }
            }

        }
    },
    configureWebpack: {
        plugins: [
            new CompressionPlugin({
                algorithm: 'gzip', // 使用gzip压缩
                test: /.js$|.html$|.css$/, // 匹配文件名
                filename: '[path][base].gz[query]', // 压缩后的文件名(保持原文件名，后缀加.gz)
                minRatio: 1, // 压缩率小于1才会压缩
                threshold: 10240, // 对超过10k的数据压缩
                deleteOriginalAssets: false, // 是否删除未压缩的源文件，谨慎设置，如果希望提供非gzip的资源，可不设置或者设置为false（比如删除打包后的gz后还可以加载到原始资源文件）
            }),
/*
            new JavaScriptObfuscator({
                compact: true,//压缩代码
                identifierNamesGenerator: 'hexadecimal',//标识符的混淆方式 hexadecimal(十六进制) mangled(短标识符)
                rotateStringArray: true,//通过固定和随机（在代码混淆时生成）的位置移动数组。这使得将删除的字符串的顺序与其原始位置相匹配变得更加困难。如果原始源代码不小，建议使用此选项，因为辅助函数可以引起注意。
                selfDefending: true,//混淆后的代码,不能使用代码美化,同时需要配置 cpmpat:true;
                stringArray: true,//删除字符串文字并将它们放在一个特殊的数组中
                disableConsoleOutput: true,
            }, [])*/
        ],
        //增加时间戳更新
        output: {
            filename: `js/[name].${timeStamp}.js`,
            chunkFilename: `js/chunk.[id].${timeStamp}.js`
        },
    },
    css: {
        //增加时间戳更新
        extract: {
            filename: `css/[name].${timeStamp}.css`,
            chunkFilename: `css/chunk[id].${timeStamp}.css`
        },
    },
}
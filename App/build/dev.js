var express = require('express'),
    webpack = require('webpack'),
    // favicon = require('express-favicon'),
    config = require('./webpack.dev.conf'),
    app = express()

var compiler = webpack(config)

// for highly stable resources
app.use('/static', express.static(config.commonPath.staticDir))

// app.use(favicon(path.join(__dirname, '../favicon.ico')));

// handle fallback for HTML5 history API
app.use(require('connect-history-api-fallback')())

// serve webpack bundle output
app.use(require('webpack-dev-middleware')(compiler, {
    noInfo: true,
    publicPath: config.output.publicPath
}))

// enable hot-reload and state-preserving
// compilation error display
app.use(require('webpack-hot-middleware')(compiler))


//设置跨域访问  
app.all('*', function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    res.header("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
    res.header("X-Powered-By", "3.2.1");
    res.header("Content-Type", "application/json;charset=utf-8");
    next();
})

app.listen(8002, function(err) {
    err && console.log(err)
})
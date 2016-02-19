module.exports = {
    config: {
        npm: {
            enabled: true
        },
        paths: {
            watched: [ 'app' ]
        },
        files:{
            javascripts: { 
                joinTo: {
                    'app.js': /^app\/src/,
                    'lib.js': /^node_modules/
                }
            },
            stylesheets: { joinTo: 'app.css' }
        },
        plugins: {
            babel:{
                presets: ['es2015', 'react'],
                ignore: [/^(node_modules|vendor|test)/],
                pattern: /\.(js|jsx)$/
            },
            autoReload:{
                enabled: true
            }
        }
    }
}
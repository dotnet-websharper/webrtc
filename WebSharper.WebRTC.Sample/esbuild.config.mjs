import { build } from 'esbuild'

await build({
  entryPoints: ['./build/WebSharper.WebRTC.Sample.js'],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: '../dist/Scripts/WebSharper.WebRTC.Sample.min.js',
  globalName: 'wsbundle'
})

console.log('Bundled: WebSharper.WebRTC.Sample.js')

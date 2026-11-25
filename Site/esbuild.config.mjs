import { build } from 'esbuild'
import { readFileSync, writeFileSync, copyFileSync, mkdirSync } from 'fs'

const name = 'Site'

// The WebSharper-generated module imports the npm `d3` package (and the
// WebSharper runtime); bundle it all into a single IIFE for the static page.
await build({
  entryPoints: [`./build/${name}.js`],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: `../dist/Scripts/${name}.min.js`,
  globalName: 'wsbundle'
})

copyFileSync('./site.css', '../dist/site.css')
copyFileSync('./topojson.v1.min.js', '../dist/topojson.v1.min.js')

mkdirSync('../dist/WorldTour', { recursive: true })
copyFileSync('./WorldTour/world-110m.json', '../dist/WorldTour/world-110m.json')
copyFileSync('./WorldTour/world-country-names.tsv', '../dist/WorldTour/world-country-names.tsv')

// Fold the WebSharper head (linked resources) into the page.
const head = readFileSync(`./build/${name}.head.html`, 'utf8')
const html = readFileSync('./index.html', 'utf8').replace('<!-- ws-head -->', head)
writeFileSync('../dist/index.html', html)

console.log('Bundled:', name)
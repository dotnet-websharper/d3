import { build } from 'esbuild'

// The WebSharper bundle (Content/Site.js) imports `d3` from npm (the binding
// declares it via NpmDependencies), so esbuild resolves and inlines it here.
await build({
  entryPoints: ['./Content/Site.js'],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: './Content/Site.bundle.js',
  globalName: 'wsbundle'
})

console.log('Bundled: Site.js -> Content/Site.bundle.js')

'use strict';

// In tests, polyfill requestAnimationFrame since jsdom doesn't provide it yet.
// We don't polyfill it in the browser--this is user's responsibility.
if (process.env.NODE_ENV === 'test') {
  require('raf').polyfill(global);
}

if (!window.Proxy || !window.Symbol) {
  throw 'In-View Rin Inspector requires "Proxy" and "Symbol" features. Most modern browsers implement those feature.';
}
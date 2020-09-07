/* eslint-disable */

// Configure public_path on the fly
// https://webpack.js.org/guides/public-path/#on-the-fly

__webpack_public_path__ = (document.querySelector('html').dataset.rinConfigPathBase.replace(/\/$/, '') || '') + '/';

// WORKAROUND: monaco-editor depends `__webpack_public_path__` variable in `window`.
window.__webpack_public_path__ = __webpack_public_path__;
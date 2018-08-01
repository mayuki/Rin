export function getMonacoLanguage(contentType: string) {
  if (contentType.startsWith('text/html')) {
    return 'html';
  }
  if (contentType.startsWith('text/css')) {
    return 'css';
  }
  if (contentType.startsWith('text/xml') || contentType.startsWith('application/xml')) {
    return 'xml';
  }
  if (contentType.startsWith('text/javascript') || contentType.startsWith('application/javascript')) {
    return 'javascript';
  }
  if (contentType.startsWith('text/json') || contentType.startsWith('application/json')) {
    return 'json';
  }

  return 'plaintext';
}

export function getContentType(headers: { [key: string]: string[] }) {
  const headerContentTypeKeys = Object.keys(headers).filter(x => x.toLowerCase() === 'content-type');
  if (headerContentTypeKeys.length === 0) {
    return null;
  }
  return headers[headerContentTypeKeys[0]][0];
}

export function isJson(contentType: string) {
  return contentType.startsWith('text/json') || contentType.startsWith('application/json');
}

export function isText(contentType: string) {
  return (
    contentType.startsWith('text/') ||
    contentType.startsWith('application/x-www-form-urlencoded') ||
    contentType.startsWith('application/xml') ||
    contentType.startsWith('application/javascript') ||
    contentType.startsWith('application/json')
  );
}

export function isImage(contentType: string) {
  return contentType.startsWith('image/');
}

export function copyTextToClipboard(value: string) {
  const inputE = document.createElement('textarea');
  inputE.value = value;
  document.body.appendChild(inputE);
  inputE.select();
  document.execCommand('copy');
  document.body.removeChild(inputE);
}

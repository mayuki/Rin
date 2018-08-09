import { BodyDataPayload, RequestRecordDetailPayload } from '../api/IRinCoreHub';
import { getContentType, isImage, isText } from '../utilities';

export function createCSharpCodeFromDetail(record: RequestRecordDetailPayload, requestBody: BodyDataPayload | null) {
  const requestContentType = getContentType(record.RequestHeaders) || '';
  const responseContentType = record.IsCompleted ? getContentType(record.ResponseHeaders) : null;
  const codeLines: string[] = [];
  const httpMethod = record.Method[0].toUpperCase() + record.Method.toLowerCase().substring(1); // GET -> Get, POST -> Post ...
  const uri = createUrl(record);

  codeLines.push('var handler = new HttpClientHandler()');
  codeLines.push('{');
  codeLines.push('	UseCookies = true,');
  codeLines.push('	CookieContainer = new CookieContainer(),');
  codeLines.push('};');

  codeLines.push('var httpClient = new HttpClient(handler);');

  codeLines.push(`var request = new HttpRequestMessage(HttpMethod.${httpMethod}, "${uri}");`);
  codeLines.push('request.Headers.ExpectContinue = false;');
  Object.keys(record.RequestHeaders).forEach(x => {
    switch (x.toLowerCase()) {
      case 'connection':
      case 'content-length':
        return;
      case 'cookie':
        codeLines.push(
          `handler.CookieContainer.SetCookies(new Uri("${uri}"), "${record.RequestHeaders[x].join(
            '\n'
          )}".Replace(";", ","));`
        );
        return;
    }

    codeLines.push(
      `request.Headers.TryAddWithoutValidation("${escapeCSharpStringLiteral(x)}", "${escapeCSharpStringLiteral(
        record.RequestHeaders[x].join('\n')
      )}");`
    );
  });

  if (requestBody != null) {
    switch (record.Method.toUpperCase()) {
      case 'POST':
      case 'PUT':
        if (requestBody.IsBase64Encoded) {
          codeLines.push(
            `request.Content = new ByteArrayContent(Convert.FromBase64String("${escapeCSharpStringLiteral(
              requestBody.Body
            )}"));`
          );
        } else {
          codeLines.push(
            `request.Content = new ByteArrayContent(new UTF8Encoding(false).GetBytes("${escapeCSharpStringLiteral(
              requestBody.Body
            )}"));`
          );
        }
        codeLines.push('');
        codeLines.push(
          `request.Content.Headers.TryAddWithoutValidation("Content-Type", "${escapeCSharpStringLiteral(
            requestContentType
          )}");`
        );
        break;
    }
  }

  codeLines.push('');
  codeLines.push(`var response = await httpClient.SendAsync(request);`);
  if (responseContentType && isText(responseContentType)) {
    codeLines.push(`await response.Content.ReadAsStringAsync().Dump();`);
  } else if (responseContentType && isImage(responseContentType)) {
    codeLines.push(`Util.Image(await response.Content.ReadAsByteArrayAsync()).Dump();`);
  } else {
    codeLines.push(`await response.Content.ReadAsByteArrayAsync().Dump();`);
  }

  return codeLines.join('\r\n');
}

function escapeCSharpStringLiteral(value: string) {
  return value
    .replace(/\\/g, '\\\\')
    .replace(/"/g, '\\"')
    .replace(/\n/g, '\\n');
}

export function createUrl(record: RequestRecordDetailPayload) {
  return (
    (record.IsHttps ? 'https' : 'http') +
    '://' +
    record.Host +
    record.Path +
    (record.QueryString != null && record.QueryString !== '' ? record.QueryString : '')
  );
}

export function createCurlFromDetail(record: RequestRecordDetailPayload, body: BodyDataPayload | null) {
  const url = createUrl(record);
  const commandParams = ['curl', url];
  const headers = Object.keys(record.RequestHeaders).map(
    x => `-H ${escapeShellLiteral(x + ': ' + record.RequestHeaders[x].join(' '))}`
  );

  const hasBody =
    (record.Method.toUpperCase() === 'POST' || record.Method.toUpperCase() === 'PUT') &&
    record.IsCompleted &&
    body != null;

  if (hasBody && body != null) {
    return commandParams
      .concat(headers)
      .concat([
        '--data-binary ' + escapeShellLiteral(body.IsBase64Encoded ? btoa(body.Body) : body.Body),
        '--compressed'
      ])
      .join(' ');
  } else {
    return commandParams
      .concat(headers)
      .concat(['--compressed'])
      .join(' ');
  }
}
function escapeShellLiteral(value: string) {
  if (value.match(/\\|'/)) {
    return "$'" + value.replace(/\\/g, '\\\\').replace(/'/g, "\\'") + "'";
  } else {
    return "'" + value + "'";
  }
}

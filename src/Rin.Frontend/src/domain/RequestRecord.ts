import { RequestRecordDetailPayload } from '../api/IRinCoreHub';
import { getContentType, isImage, isText } from '../utilities';

export function createCSharpCodeFromDetail(record: RequestRecordDetailPayload) {
  const responseContentType = record.IsCompleted ? getContentType(record.ResponseHeaders) : null;
  const codeLines: string[] = [];
  const httpMethod = record.Method[0].toUpperCase() + record.Method.toLowerCase().substring(1); // GET -> Get, POST -> Post ...
  const uri = `http://localhost:5000${record.Path}`;

  codeLines.push('var httpClient = new HttpClient();');

  codeLines.push(`var request = new HttpRequestMessage(HttpMethod.${httpMethod}, "${uri}");`);
  Object.keys(record.RequestHeaders).forEach(x => {
    codeLines.push(
      `request.Headers.TryAddWithoutValidation("${x.replace(/"/g, '\\"')}", "${record.RequestHeaders[x]
        .join('\n')
        .replace(/"/g, '\\"')}");`
    );
  });
  switch (record.Method.toUpperCase()) {
    case 'POST':
    case 'PUT':
      codeLines.push(`request.Content = new ByteArrayContent(new byte[0]);`);
      break;
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

const FAVICON_API = 'https://www.google.com/s2/favicons?domain=';

export function extractDomain(url: string): string {
  if (!url) return '';
  try {
    const u = url.startsWith('http') ? url : `https://${url}`;
    return new URL(u).hostname;
  } catch {
    return '';
  }
}

export function getFaviconUrl(url: string): string {
  const domain = extractDomain(url);
  return domain ? `${FAVICON_API}${domain}&sz=64` : '';
}

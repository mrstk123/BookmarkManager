import { Bookmark } from '../../models/bookmark.model';

export function filterBookmarks(bookmarks: Bookmark[], query: string): Bookmark[] {
  if (!query.trim()) return bookmarks;
  const q = query.toLowerCase();
  return bookmarks.filter(b =>
    b.title.toLowerCase().includes(q) ||
    b.url.toLowerCase().includes(q) ||
    (b.folderName?.toLowerCase().includes(q)) ||
    b.tags.some((t: string) => t.toLowerCase().includes(q))
  );
}

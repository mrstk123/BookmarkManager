export interface Bookmark {
    id: number;
    title: string;
    url: string;
    folderName?: string;
    folderId?: number;
    tags: string[];
    color?: string;
    isFavorite: boolean;
    iconUrl?: string;
}
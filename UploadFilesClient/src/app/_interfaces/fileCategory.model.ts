import { FileInfo } from './file.info.model';

export interface FileCategory {
    contentType: string
    files: FileInfo[]
}
export class PaginationModel {
    public page: number;
    public pageSize: number;
    public totalRecords: number;
    public totalPages: number;

    constructor() {
        this.page = 1;
        this.pageSize = 10;
        this.totalRecords = 0;
        this.totalPages = 0;
    }
}
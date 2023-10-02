<template>
    <div>
        <div class="container-sm">
            <div class="caption fw-bold">Your posted books</div>

            <div class="row">
                <div class="col-sm-4">
                    <search identifier="titleFilter" filterPlaceholder="Search for Title" label="Title: "
                        :filterVal="fetchParams.titleFilter" @change-filter-value-event="changeFilterValueEventHandler">
                    </search>
                </div>
                <div class="col-sm-4">
                    <search identifier="authorsFilter" filterPlaceholder="Search for Authors" label="Authors: "
                        :filterVal="fetchParams.authorsFilter" @change-filter-value-event="changeFilterValueEventHandler">
                    </search>
                </div>
                <div class="col-sm-4">
                    <search identifier="isbnFilter" filterPlaceholder="Search for ISBN" label="ISBN: "
                        :filterVal="fetchParams.isbnFilter" @change-filter-value-event="changeFilterValueEventHandler">
                    </search>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-4">
                    <dropdown identifier="orderByFilter" :values="orderByFilterValues"
                        :defaultValue="fetchParams.orderByFilter" label="Order by: "
                        @change-filter-value-event="changeFilterValueEventHandler">
                    </dropdown>
                </div>
                <div class="col-sm-4">
                    <dropdown identifier="sortFilter" :values="sortFilterValues" :defaultValue="fetchParams.sortFilter"
                        label="Sort: " @change-filter-value-event="changeFilterValueEventHandler">
                    </dropdown>
                </div>
                <div class="col-sm-4">
                    <dropdown identifier="itemsPerPageFilter" :values="itemsPerPageFilterValues"
                        :defaultValue="fetchParams.itemsPerPageFilter" label="Items per page: "
                        @change-filter-value-event="changeFilterValueEventHandler">
                    </dropdown>
                </div>
            </div>

            <div class="row d-flex justify-content-center">
                <div class="col-sm-4">
                    <button id="addBook" @click="onAddBookClickEventHandler" type="button" class="btn btn-dark"
                        style="margin-top: 1em;">Upload book</button>
                </div>
            </div>

            <div style="margin: 2rem"></div>

            <div class="row">
                <book v-for="book in booksData" :key="book.id" :book="book"
                    @show-book-details-event="onShowBookDetailsEventHandler"
                    @hide-book-details-event="onHideBookDetailsEventHandler"
                    @book-download-error-event="onBookDownloadErrorEventHandler"
                    @delete-book-event="onDeleteBookEventHandler" @edit-book-event="onEditBookEventHandler">
                </book>

                <span v-if="booksData.length == 0" class="d-flex justify-content-center">No records to view</span>
            </div>

            <div v-if="booksData.length !== 0" class="d-flex justify-content-center">
                <pagination :total="totalBooks" :items-per-page="fetchParams.itemsPerPageFilter" :page="fetchParams.page"
                    @page-changed-event="onPageChangeEventHandler"></pagination>
            </div>
        </div>

        <!-- this hidden link is used to call info-modal  -->
        <a href="#" ref="hiddenLink" id="hiddenLink" class="hiddenLink" data-bs-toggle="modal" data-bs-target="#info"></a>

        <Teleport to="body">
            <info-modal identifier="info" :message="infoModalMessage"></info-modal>
        </Teleport>

        <!-- this hidden link is used to call confirmation-modal  -->
        <a href="#" ref="hiddenLinkConfirmDelete" id="hiddenLinkConfirmDelete" class="hiddenLink" data-bs-toggle="modal"
            data-bs-target="#confirm"></a>

        <Teleport to="body">
            <confirmation-modal identifier="confirm" title="Delete book" :message="modalConfirmMsg"
                @confirm-event="onConfirmDeleteEventHandler"></confirmation-modal>
        </Teleport>

        <!-- this hidden link is used to call editBook modal  -->
        <a href="#" ref="hiddenLinkEditBook" id="hiddenLinkEditBook" class="hiddenLink" data-bs-toggle="modal"
            data-bs-target="#editBook"></a>

        <Teleport to="body">
            <!-- Modal edit -->
            <div class="modal fade" id="editBook" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
                aria-labelledby="editBookTitle" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5" id="editBookTitle"> Edit book
                            </h1>
                            <button id="editBookCloseBtn" type="button" class="btn-close" data-bs-dismiss="modal"
                                aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                <label for="editTitle" class="form-label">Title</label>
                                <input v-model="editBook.title" type="text" class="form-control" id="editTitle"
                                    placeholder="Title">
                                <div v-if="editBook.titleValidationMsg != ''" class="form-text">
                                    <span v-html="editBook.titleValidationMsg"></span>
                                </div>
                            </div>
                            <div class="mb-3">
                                <label for="editAuthors" class="form-label">Authors</label>
                                <input v-model="editBook.authors" type="text" class="form-control" id="editAuthors"
                                    placeholder="Authors">
                                <div v-if="editBook.authorsValidationMsg != ''" class="form-text">
                                    <span v-html="editBook.authorsValidationMsg"></span>
                                </div>
                            </div>
                            <div class="mb-3">
                                <label for="editIsbn" class="form-label">ISBN</label>
                                <input v-model="editBook.isbn" type="text" class="form-control" id="editIsbn"
                                    placeholder="ISBN">
                            </div>
                            <div class="mb-3">
                                <label for="editDescription" class="form-label">Description</label>
                                <textarea v-model="editBook.description" class="form-control" id="editDescription"
                                    placeholder="Description"></textarea>
                            </div>
                            <div class="mb-3">
                                <label for="editCover" class="form-label">Cover</label>
                                <input @change="editHandleCoverUpload" type="file" class="form-control" id="editCover"
                                    placeholder="Cover">
                            </div>
                            <div class="mb-3">
                                <label for="editFile" class="form-label">Book</label>
                                <input @change="editHandleBookUpload" type="file" class="form-control" id="editFile"
                                    placeholder="Book">
                                <div v-if="editBook.fileValidationMsg != ''" class="form-text">
                                    <span v-html="editBook.fileValidationMsg"></span>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button @click.prevent="onClickEditButtonEventHandler" type="button"
                                class="btn btn-dark">Edit</button>
                            <button type="button" class="btn btn-dark" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </Teleport>

        <!-- this hidden link is used to call createBook modal  -->
        <a href="#" ref="hiddenLinkCreateBook" id="hiddenLinkCreateBook" class="hiddenLink" data-bs-toggle="modal"
            data-bs-target="#createBook"></a>

        <Teleport to="body">
            <!-- Modal create -->
            <div class="modal fade" id="createBook" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
                aria-labelledby="createBookTitle" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5" id="createBookTitle"> Upload book </h1>
                            <button id="createBookCloseBtn" type="button" class="btn-close" data-bs-dismiss="modal"
                                aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                <label for="createTitle" class="form-label">Title</label>
                                <input v-model="createBook.title" type="text" class="form-control" id="createTitle"
                                    placeholder="Title">
                                <div v-if="createBook.titleValidationMsg" class="form-text">
                                    <span v-html="createBook.titleValidationMsg"></span>
                                </div>
                            </div>
                            <div class="mb-3">
                                <label for="createAuthors" class="form-label">Authors</label>
                                <input v-model="createBook.authors" type="text" class="form-control" id="createAuthors"
                                    placeholder="Authors">
                                <div v-if="createBook.authorsValidationMsg" class="form-text">
                                    <span v-html="createBook.authorsValidationMsg"></span>
                                </div>
                            </div>
                            <div class="mb-3">
                                <label for="createIsbn" class="form-label">ISBN</label>
                                <input v-model="createBook.isbn" type="text" class="form-control" id="createIsbn"
                                    placeholder="ISBN">
                            </div>
                            <div class="mb-3">
                                <label for="createDescription" class="form-label">Description</label>
                                <textarea v-model="createBook.description" class="form-control" id="createDescription"
                                    placeholder="Description"></textarea>
                            </div>
                            <div class="mb-3">
                                <label for="createCover" class="form-label">Cover</label>
                                <input @change="createHandleCoverUpload" type="file" class="form-control" id="createCover"
                                    placeholder="Cover">
                            </div>
                            <div class="mb-3">
                                <label for="createFile" class="form-label">Book</label>
                                <input @change="createHandleBookUpload" type="file" class="form-control" id="createFile"
                                    placeholder="Book">
                                <div v-if="createBook.fileValidationMsg != ''" class="form-text">
                                    <span v-html="createBook.fileValidationMsg"></span>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button @click.prevent="onClickAddButton" type="button" class="btn btn-dark">Add</button>
                            <button type="button" class="btn btn-dark" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </Teleport>
    </div>
</template>
      
<script>
import router from '../router';
import SearchComponent from '../components/SearchComponent.vue';
import DropdownComponent from '../components/DropdownComponent.vue';
import BookComponent from '../components/BookComponent.vue';
import { postAuthorized, deleteAuthorized, putAuthorized } from '../services/CrudService';
import getLoggedUser from '../mixins/getLoggedUser.js';
import PaginationComponent from '../components/PaginationComponent.vue';
import InfoModalComponent from '../components/InfoModalComponent.vue';
import ConfirmationModalComponent from '../components/ConfirmationModalComponent.vue';
import { checkIfUserIsAuthenticated } from '@/services/AuthenticationService';

export default {
    name: 'AdminPage',

    components: {
        Search: SearchComponent,
        Dropdown: DropdownComponent,
        Book: BookComponent,
        Pagination: PaginationComponent,
        InfoModal: InfoModalComponent,
        ConfirmationModal: ConfirmationModalComponent
    },

    data() {
        return {
            booksData: [],
            totalBooks: 0,

            fetchParams: {
                titleFilter: '',
                authorsFilter: '',
                isbnFilter: '',
                orderByFilter: 'Title',
                sortFilter: 'Ascending',
                itemsPerPageFilter: '5',
                page: 1
            },

            orderByFilterValues: ['Title', 'Authors', 'ISBN'],
            sortFilterValues: ['Ascending', 'Descending'],
            itemsPerPageFilterValues: ['5', '10', '20', '50'],

            infoModalMessage: '',
            modalConfirmMsg: `
                <p>
                    The book will not be deleted from the database, but it will no 
                    longer be associated with you. The book will be associated with 
                    the "anonymous" user.
                    <br />
                    Do you confirm?
                </p>
            `,

            urls: {
                getPagedSortedAndFilteredBooksUrl: "api/book/getPagedAdminPage",
                deleteUrl: "api/book/delete",
                editUrl: "api/book/update",
                createUrl: "api/book/create"
            },

            bookIdToDelete: null,
            bookIdToEdit: null,

            editBook: {
                title: null,
                authors: null,
                isbn: null,
                description: null,
                cover: null,
                coverName: null,
                isCoverEdited: false,
                file: null,
                fileName: null,
                isFileEdited: false,

                titleValidationMsg: "",
                authorsValidationMsg: "",
                fileValidationMsg: ""
            },

            createBook: {
                title: null,
                authors: null,
                isbn: null,
                description: null,
                cover: null,
                coverName: null,
                file: null,
                fileName: null,

                titleValidationMsg: "",
                authorsValidationMsg: "",
                fileValidationMsg: ""
            }
        }
    },

    mixins: [getLoggedUser],

    created() {
        this.fetchParams.page = parseInt(this.$route.params.page);

        let queryParams = this.$route.query;
        let defaultQuery = this.$route.meta.defaultQuery;

        this.fetchParams.titleFilter = queryParams.title === undefined ? defaultQuery.title : queryParams.title;
        this.fetchParams.authorsFilter = queryParams.authors === undefined ? defaultQuery.authors : queryParams.authors;
        this.fetchParams.isbnFilter = queryParams.isbn === undefined ? defaultQuery.isbn : queryParams.isbn;
        this.fetchParams.orderByFilter = queryParams.orderBy === undefined ? defaultQuery.orderBy : queryParams.orderBy;
        this.fetchParams.sortFilter = queryParams.sort === undefined ? defaultQuery.sort : queryParams.sort;
        this.fetchParams.itemsPerPageFilter = queryParams.items === undefined ? defaultQuery.items : queryParams.items;
        // fix dropdowns if inexistent option is added from request param
        if (this.orderByFilterValues.includes(this.fetchParams.orderByFilter) === false) {
            this.fetchParams.orderByFilter = defaultQuery.orderBy;
        }
        if (this.sortFilterValues.includes(this.fetchParams.sortFilter) === false) {
            this.fetchParams.sortFilter = defaultQuery.sort;
        }
        if (this.itemsPerPageFilterValues.includes(this.fetchParams.itemsPerPageFilter) === false) {
            this.fetchParams.itemsPerPageFilter = defaultQuery.items;
        }
    },

    mounted() {
        window.addEventListener('hidden.bs.modal', () => {
            this.bookIdToDelete = null;
            this.bookIdToEdit = null;
        });

        this.fetchBooks();
    },

    methods: {
        onPageChangeEventHandler(page) {
            this.fetchParams.page = page;
            this.$route.params.page = page;

            this.pushRoute();
        },

        async changeFilterValueEventHandler(identifier, filterValue) {
            let queryParams = this.$route.query;

            switch (identifier) {
                case 'titleFilter':
                    this.fetchParams.titleFilter = filterValue;
                    queryParams.title = filterValue;
                    this.updateRoute('title');
                    break;
                case 'authorsFilter':
                    this.fetchParams.authorsFilter = filterValue;
                    queryParams.authors = filterValue;
                    this.updateRoute('authors');
                    break;
                case 'isbnFilter':
                    this.fetchParams.isbnFilter = filterValue;
                    queryParams.isbn = filterValue;
                    this.updateRoute('isbn');
                    break;
                case 'isReadFilter':
                    this.fetchParams.isReadFilter = filterValue;
                    queryParams.isRead = filterValue;
                    this.updateRoute('isRead');
                    break;
                case 'orderByFilter':
                    this.fetchParams.orderByFilter = filterValue;
                    queryParams.orderBy = filterValue;
                    this.updateRoute('orderBy');
                    break;
                case 'sortFilter':
                    this.fetchParams.sortFilter = filterValue;
                    queryParams.sort = filterValue;
                    this.updateRoute('sort');
                    break;
                case 'itemsPerPageFilter':
                    this.fetchParams.itemsPerPageFilter = filterValue;
                    queryParams.items = filterValue;
                    this.updateRoute('items');
                    break;
            }

            this.fetchParams.page = 1;
            this.$route.params.page = this.fetchParams.page;

            this.pushRoute();
        },

        updateRoute(fieldName) {
            let queryParams = this.$route.query;
            let defaultQuery = this.$route.meta.defaultQuery;

            if (queryParams[fieldName] === '' || queryParams[fieldName] === defaultQuery[fieldName]) {
                delete queryParams[fieldName]
            }
        },

        async pushRoute(scroll = true) {
            router.push({ name: this.$route.name, params: this.$route.params, query: this.$route.query });

            await this.fetchBooks(scroll);
        },

        async fetchBooks(scroll) {
            let email = await this.getLoggedUserEmail();

            const payload = {
                pageSize: parseInt(this.fetchParams.itemsPerPageFilter),
                page: this.fetchParams.page,
                orderBy: this.fetchParams.orderByFilter,
                orderDirection: this.fetchParams.sortFilter,
                filters: {
                    title: this.fetchParams.titleFilter,
                    authors: this.fetchParams.authorsFilter,
                    isbn: this.fetchParams.isbnFilter
                },
                userEmail: email
            };

            const response = await postAuthorized(this.urls.getPagedSortedAndFilteredBooksUrl, payload);

            if (response.status === true) {
                this.booksData = response.data.items;
                this.totalBooks = response.data.totalItems;
            } else {
                this.infoModalMessage = response.message;
                this.$refs.hiddenLink.click();
            }

            if (scroll) {
                window.scroll(0, 0);
            }
        },

        onShowBookDetailsEventHandler(bookId) {
            router.push({ name: 'BookDetailsForAdminPage', params: { id: bookId }, query: this.$route.query });
        },

        onHideBookDetailsEventHandler() {
            router.push({ name: 'AdminPage', params: { page: this.fetchParams.page }, query: this.$route.query });
        },

        async onAddBookClickEventHandler() {
            this.resetCreateBook();
            this.resetValidationCreateBook();

            this.$refs.hiddenLinkCreateBook.click();
        },

        createHandleCoverUpload(event) {
            const reader = new FileReader();
            reader.onload = (e) => {
                this.createBook.cover = e.target.result;
                this.createBook.coverName = event.target.files[0].name;
            }

            if (event.target.files[0]) {
                reader.readAsDataURL(event.target.files[0]);
            } else {
                this.createBook.cover = null;
                this.createBook.coverName = null;
            }
        },

        createHandleBookUpload(event) {
            const reader = new FileReader();
            reader.onload = (e) => {
                this.createBook.file = e.target.result;
                this.createBook.fileName = event.target.files[0].name;
            }
            if (event.target.files[0]) {
                reader.readAsDataURL(event.target.files[0]);
            } else {
                this.createBook.file = null;
                this.createBook.fileName = null;
            }
        },

        async onClickAddButton() {
            let createBookCloseBtn = document.getElementById('createBookCloseBtn');

            let isAuthenticated = await checkIfUserIsAuthenticated();

            if (isAuthenticated === false) {
                createBookCloseBtn.click();
                router.push({ name: 'LoginPage' });
            }

            let email = await this.getLoggedUserEmail();

            const payload = {
                Title: this.adjustInput(this.createBook.title),
                Authors: this.adjustInput(this.createBook.authors),
                ISBN: this.adjustInput(this.createBook.isbn),
                Description: this.adjustInput(this.createBook.description),
                Cover: this.createBook.cover,
                CoverName: this.createBook.coverName,
                File: this.createBook.file,
                FileName: this.createBook.fileName,
                AddedByEmail: email
            }

            const response = await postAuthorized(this.urls.createUrl, payload);

            if (response.status == 400 && response.errors) {
                if ("Title" in response.errors) {
                    this.createBook.titleValidationMsg = response.errors.Title.join('<br/>');
                }

                if ("Authors" in response.errors) {
                    this.createBook.authorsValidationMsg = response.errors.Authors.join('<br/>');
                }

                if ("File" in response.errors) {
                    this.createBook.fileValidationMsg = response.errors.File.join('<br/>');
                }

                return;
            }

            if (response.status == true) {
                createBookCloseBtn.click();

                this.infoModalMessage = "Book has been successfully uploaded.";
                this.$refs.hiddenLink.click();

                this.pushRoute();
            } else {
                this.infoModalMessage = response.message;
                this.$refs.hiddenLink.click();
            }
        },

        resetValidationCreateBook() {
            this.createBook.titleValidationMsg = "";
            this.createBook.authorsValidationMsg = "",
                this.createBook.fileValidationMsg = "";
        },

        resetCreateBook() {
            this.createBook.title = null;
            this.createBook.authors = null;
            this.createBook.isbn = null;
            this.createBook.description = null;
            this.createBook.cover = null;
            this.createBook.coverName = null;
            this.createBook.file = null;
            this.createBook.fileName = null;

            document.getElementById('createCover').value = null;
            document.getElementById('createFile').value = null;

            document.getElementById("createDescription").style.height = "";
        },

        async onEditBookEventHandler(bookId) {
            this.resetEditBook();
            this.resetValidationEditBook();

            let book = this.booksData.find(book => book.id === bookId);
            this.bookIdToEdit = book.id;

            this.editBook.title = book.title;
            this.editBook.authors = book.authors;
            this.editBook.isbn = book.isbn;
            this.editBook.description = book.description;

            this.editBook.coverName = book.coverName !== null ? book.coverName : "";
            const editCover = document.getElementById("editCover");
            const fakeCoverFile = new File(['empty'], this.editBook.coverName, {
                type: 'text/plain',
                lastModified: new Date(),
            });
            const dataTransfer1 = new DataTransfer();
            dataTransfer1.items.add(fakeCoverFile);
            editCover.files = dataTransfer1.files;
            if (editCover.webkitEntries.length) {
                editCover.dataset.file = `${dataTransfer1.files[0].name}`;
            }

            this.editBook.fileName = book.fileName;
            const editFile = document.getElementById("editFile");
            const fakeFile = new File(['empty'], this.editBook.fileName, {
                type: 'text/plain',
                lastModified: new Date(),
            });
            const dataTransfer2 = new DataTransfer();
            dataTransfer2.items.add(fakeFile);
            editFile.files = dataTransfer2.files;
            if (editFile.webkitEntries.length) {
                editFile.dataset.file = `${dataTransfer2.files[0].name}`;
            }
            this.editBook.file = 'nothing';

            this.$refs.hiddenLinkEditBook.click();
        },

        editHandleCoverUpload(event) {
            const reader = new FileReader();
            reader.onload = (e) => {
                this.editBook.cover = e.target.result;
                this.editBook.coverName = event.target.files[0].name;
                this.editBook.isCoverEdited = true;
            }

            if (event.target.files[0]) {
                reader.readAsDataURL(event.target.files[0]);
            } else {
                this.editBook.cover = null;
                this.editBook.coverName = null;
                this.editBook.isCoverEdited = true;
            }
        },

        editHandleBookUpload(event) {
            const reader = new FileReader();
            reader.onload = (e) => {
                this.editBook.file = e.target.result;
                this.editBook.fileName = event.target.files[0].name;
                this.editBook.isFileEdited = true;
            }
            if (event.target.files[0]) {
                reader.readAsDataURL(event.target.files[0]);
            } else {
                this.editBook.file = null;
                this.editBook.fileName = null;
                this.editBook.isFileEdited = true;
            }
        },

        async onClickEditButtonEventHandler() {
            let editBookCloseBtn = document.getElementById('editBookCloseBtn');

            let isAuthenticated = await checkIfUserIsAuthenticated();

            if (isAuthenticated === false) {
                editBookCloseBtn.click();
                router.push({ name: 'LoginPage' });
            }

            const payload = {
                Id: this.bookIdToEdit,
                Title: this.adjustInput(this.editBook.title),
                Authors: this.adjustInput(this.editBook.authors),
                ISBN: this.adjustInput(this.editBook.isbn),
                Description: this.adjustInput(this.editBook.description),
                Cover: this.editBook.cover,
                CoverName: this.editBook.coverName,
                IsCoverEdited: this.editBook.isCoverEdited,
                File: this.editBook.file,
                FileName: this.editBook.fileName,
                IsFileEdited: this.editBook.isFileEdited
            }

            const response = await putAuthorized(this.urls.editUrl, payload);

            if (response.status == 400 && response.errors) {
                if ("Title" in response.errors) {
                    this.editBook.titleValidationMsg = response.errors.Title.join('<br/>');
                }

                if ("Authors" in response.errors) {
                    this.editBook.authors = response.errors.Authors.join('<br/>');
                }

                if ("File" in response.errors) {
                    this.editBook.fileValidationMsg = response.errors.File.join('<br/>');
                }

                return;
            }

            if (response.status == true) {
                editBookCloseBtn.click();

                this.infoModalMessage = "Book has been successfully updated.";
                this.$refs.hiddenLink.click();

                this.pushRoute(false);
            } else {
                this.infoModalMessage = response.message;
                this.$refs.hiddenLink.click();
            }
        },

        resetValidationEditBook() {
            this.editBook.titleValidationMsg = "";
            this.editBook.authorsValidationMsg = "",
                this.editBook.fileValidationMsg = "";
        },

        resetEditBook() {
            this.editBook.title = null;
            this.editBook.authors = null;
            this.editBook.isbn = null;
            this.editBook.description = null;
            this.editBook.cover = null;
            this.editBook.coverName = null;
            this.editBook.isCoverEdited = false;
            this.editBook.file = null;
            this.editBook.fileName = null;
            this.editBook.isFileEdited = false;

            document.getElementById('editCover').value = null;
            document.getElementById('editFile').value = null;

            document.getElementById("editDescription").style.height = "";
        },

        adjustInput(value) {
            return value !== null && value.trim() !== "" ? value : null;
        },

        onDeleteBookEventHandler(bookId) {
            this.bookIdToDelete = bookId;
            this.$refs.hiddenLinkConfirmDelete.click();
        },

        async onConfirmDeleteEventHandler() {
            const url = this.urls.deleteUrl + '/' + this.bookIdToDelete;

            const response = await deleteAuthorized(url);

            if (response.status === true) {
                this.infoModalMessage = "The book has been successfully associated with <b>anonymous</b> user.";
                this.$refs.hiddenLink.click();

                if (this.$route.name === 'BookDetailsForAdminPage' && this.bookIdToDelete === parseInt(this.$route.params.id)) {
                    this.$route.name = 'AdminPage';
                    delete this.$route.params.id;
                    // router.push({ name: 'AdminPage', params: { page: this.fetchParams.page }, query: this.$route.query });
                }

                if (this.booksData.length % parseInt(this.fetchParams.itemsPerPageFilter) === 1 && this.fetchParams.page > 1) {
                    this.fetchParams.page -= 1;
                    this.$route.params.page = this.fetchParams.page;
                }

                this.pushRoute(false);
            } else {
                this.infoModalMessage = response.message;
                this.$refs.hiddenLink.click();
            }
        },

        onBookDownloadErrorEventHandler(message) {
            this.infoModalMessage = message;
            this.$refs.hiddenLink.click();
        }
    }
};
</script>
      
<style scoped>
.list-group {
    --bs-list-group-border-width: 0px;
}

.list-group-item {
    margin-bottom: 20px;
}

.caption {
    padding-top: 0.5rem;
    padding-bottom: 0.5rem;
    color: #6c757d;
    text-align: left;
}

.col-sm-4 .btn {
    width: 100%;
}

.hiddenLink {
    display: none;
}

.btn-close:focus,
.btn-dark:focus,
.form-control:focus,
.form-select:focus {
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
    border-color: #6c757d;
}
</style>
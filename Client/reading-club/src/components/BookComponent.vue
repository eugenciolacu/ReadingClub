<template>
    <ul class="list-group list-group-light mb-4">
        <li class="list-group-item d-flex justify-content-between align-items-center">
            <div class="d-flex align-items-start">
                <img :src="coverImage" class="img-thumbnail" alt="cover" style="width: 200px;" />
                <div class="vr ms-3"></div>
                <div class="ms-3">
                    <div class="block">
                        <p class="fw-bold mb-1">Title:</p>
                        <p class="mb-0">{{ book.title }}</p>
                    </div>

                    <div class="block">
                        <p class="fw-bold mb-1">Authors:</p>
                        <p class="mb-0">{{ book.authors }}</p>
                    </div>

                    <div class="block">
                        <p class="fw-bold mb-1">ISBN:</p>
                        <p class="mb-0">{{ isbn }}</p>
                    </div>

                    <div v-if="book.id != $route.params.id" class="block">
                        <button @click="onShowDetailsClickEventHandler" type="button" class="btn btn-dark">Show
                            details</button>
                        <button v-if="book.isInReadingList === false && isUsedFromSearchForBooksPage"
                            @click="onAddToReadingListClickEventHandler" type="button" class="btn btn-dark">Add to reading
                            list</button>
                        <button v-if="book.isInReadingList === true && isUsedFromReadingListPage"
                            @click="onRemoveFromReadingListClickEventHandler" type="button" class="btn btn-dark">Remove from
                            reading list</button>
                        <button v-if="book.isInReadingList === true && book.isRead === false && isUsedFromReadingListPage"
                            @click="onMarkAsReadOrUnreadClickEventHandler" type="button" class="btn btn-dark">Mark as
                            read</button>
                        <button v-if="book.isInReadingList === true && book.isRead === true && isUsedFromReadingListPage"
                            @click="onMarkAsReadOrUnreadClickEventHandler" type="button" class="btn btn-dark">Mark as
                            unread</button>
                        <button v-if="isUsedFromAdminPage" @click="onEditEventHandler" type="button" class="btn btn-dark">
                            Edit</button>
                        <button v-if="isUsedFromAdminPage" @click="onDeleteEventHandler" type="button" class="btn btn-dark">
                            Delete</button>
                    </div>

                    <router-view v-if="book.id == $route.params.id">
                        <template v-slot>
                            <div class="block">
                                <p class="fw-bold mb-1">Dscription:</p>
                                <p class="mb-0">{{ description }}</p>
                            </div>

                            <div class="block">
                                <p class="fw-bold mb-1">Download:</p>
                                <a @click="onDownloadClickEventHandler" class="link-dark fw-bold"
                                    href="javascript:void(0);"> {{ book.fileName }} </a>
                            </div>

                            <div class="block">
                                <p class="fw-bold mb-1">Posted by:</p>
                                <p class="mb-0">{{ book.addedByUserName }}</p>
                            </div>

                            <div class="block">
                                <button @click="onHideDetailsClickEventHandler" type="button" class="btn btn-dark">Hide
                                    details</button>
                                <button v-if="book.isInReadingList === false && isUsedFromSearchForBooksPage"
                                    @click="onAddToReadingListClickEventHandler" type="button" class="btn btn-dark">Add to
                                    reading list</button>
                                <button v-if="book.isInReadingList === true && isUsedFromReadingListPage"
                                    @click="onRemoveFromReadingListClickEventHandler" type="button"
                                    class="btn btn-dark">Remove
                                    from reading list</button>
                                <button
                                    v-if="book.isInReadingList === true && book.isRead === false && isUsedFromReadingListPage"
                                    @click="onMarkAsReadOrUnreadClickEventHandler" type="button" class="btn btn-dark">Mark
                                    as read</button>
                                <button
                                    v-if="book.isInReadingList === true && book.isRead === true && isUsedFromReadingListPage"
                                    @click="onMarkAsReadOrUnreadClickEventHandler" type="button" class="btn btn-dark">Mark
                                    as unread</button>
                                <button v-if="isUsedFromAdminPage" @click="onEditEventHandler" type="button"
                                    class="btn btn-dark">Edit</button>
                                <button v-if="isUsedFromAdminPage" @click="onDeleteEventHandler" type="button"
                                    class="btn btn-dark">Delete</button>
                            </div>
                        </template>
                    </router-view>
                </div>
            </div>
        </li>
    </ul>
</template>
    
<script>
import noImageAvailable from '../assets/no-image-available.jpg';
import { postAuthorized } from '../services/CrudService';

export default {
    name: 'BookComponent',

    props: {
        book: {
            type: Object,
            required: true
        }
    },

    data() {
        return {
            getBookForDownloadUrl: 'api/book/getBookForDownload'
        }
    },

    computed: {
        isUsedFromSearchForBooksPage() {
            return this.$route.name === 'SearchForBooksPage' || this.$route.name === 'BookDetailsForSearchPage';
        },

        isUsedFromReadingListPage() {
            return this.$route.name === 'ReadingListPage' || this.$route.name === 'BookDetailsForReadingListPage';
        },

        isUsedFromAdminPage() {
            return this.$route.name === 'AdminPage' || this.$route.name === 'BookDetailsForAdminPage'
        },

        coverImage() {
            if (this.book.coverMime && this.book.cover) {
                return this.book.coverMime + ',' + this.book.cover;
            }
            return noImageAvailable;
        },

        isbn() {
            return this.book.isbn ? this.book.isbn : "ISBN not available";
        },

        description() {
            return this.book.description ? this.book.description : "Description not available";
        }
    },

    emits: ['show-book-details-event', 'hide-book-details-event',
        'add-book-to-reading-list-event', 'remove-book-from-reading-list-event',
        'mark-book-as-read-or-unread-event', 'book-download-error-event',
        'delete-book-event', 'edit-book-event'],

    methods: {
        async onDownloadClickEventHandler() {
            const payload = {
                id: this.book.id
            }

            const response = await postAuthorized(this.getBookForDownloadUrl, payload);

            if (response.status === true) {
                try {
                    const base64Data = response.data;
                    const dataUri = `data:application/octet-stream;base64,${base64Data}`;

                    const link = document.createElement('a');
                    link.href = dataUri;
                    link.download = this.book.fileName;
                    link.click();
                } catch (error) {
                    this.$emit('book-download-error-event', 'An error occurred during processing, cannot retrieve selected book.');
                }
            } else {
                this.$emit('book-download-error-event', response.message);
            }
        },

        onShowDetailsClickEventHandler() {
            this.$emit('show-book-details-event', this.book.id);
        },

        onHideDetailsClickEventHandler() {
            this.$emit('hide-book-details-event');
        },

        onAddToReadingListClickEventHandler() {
            this.$emit('add-book-to-reading-list-event', this.book.id);
        },

        onRemoveFromReadingListClickEventHandler() {
            this.$emit('remove-book-from-reading-list-event', this.book.id);
        },

        onMarkAsReadOrUnreadClickEventHandler() {
            this.$emit('mark-book-as-read-or-unread-event', this.book.id, !this.book.isRead);
        },

        onDeleteEventHandler() {
            this.$emit('delete-book-event', this.book.id);
        },

        onEditEventHandler() {
            this.$emit('edit-book-event', this.book.id);
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
    padding: 0px;
}

.block {
    margin-bottom: 1rem;
}

.block:last-child {
    margin-bottom: 0rem;
}

button {
    margin-right: 20px;
    margin-bottom: 10px;
}
</style>
    
<template>
    <label :for="identifier">{{label}}</label>
    <div class="input-group">
        <input :id="identifier" type="text" class="form-control" 
            :placeholder="filterPlaceholder"
            v-model="filterValue"
            @keyup.esc = "onKeyUp"
            @change="onChange"
        >
        
        <button v-if="filterValue.length > 0" class="btn btn-dark" type="button"
            @click="onKeyUp"
        >
            X
        </button>
    </div>
</template>
    
<script>

export default {
    name: 'SearchComponent',

    props: {
        identifier: {
            type: String,
            required: true
        },

        filterPlaceholder: {
            type: String,
            required: true
        },

        label: {
            type: String,
            required: true
        },

        filterVal: {
            type: String,
            required: true
        }
    },

    data() {
        return {
            filterValue: ""
        }
    },

    emits: ['change-filter-value-event'],

    created() {
        this.filterValue = this.filterVal;
    },

    methods: {
        onChange() {
            this.$emit('change-filter-value-event', this.identifier, this.filterValue);
        },

        onKeyUp(){
            this.filterValue = "";
            this.onChange();
        }
    }
};
</script>
  
<style scoped>
.btn-dark:focus,
.form-control:focus {
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
    border-color: #6c757d;
}
</style>
    
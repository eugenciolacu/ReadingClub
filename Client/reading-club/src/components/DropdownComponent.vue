<template>
    <div>
        <label :for="identifier">{{label}}</label>
        <select :id="identifier" v-model="selectedValue" @change="onChange" class="form-select">
            <option v-for="item in this.values" :key="item" :value="item">{{ item }}</option>
        </select>
    </div>
</template>
    
<script>

export default {
    name: 'DropdownComponent',

    props: {
        identifier: {
            type: String,
            required: true
        },

        values: {
            type: Array,
            required: true
        },

        defaultValue: {
            type: String,
            required: true
        },

        label: {
            type: String,
            required: true
        }
    },

    data() {
        return {
            selectedValue: this.defaultValue,
        }
    },

    emits: ['change-filter-value-event'],

    methods: {
        onChange() {
            this.$emit('change-filter-value-event', this.identifier, this.selectedValue);
        }
    }
};
</script>
  
<style scoped>
.form-select:focus {
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.15), 0 1px 1px rgba(0, 0, 0, 0.075);
    border-color: #6c757d;
}
</style>
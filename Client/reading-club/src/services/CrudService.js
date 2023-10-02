import { getToken } from '../services/AuthenticationService'

const ApiUrl = 'https://localhost:7048/';

export async function postAnonymous(partialUrl, payload) {
  const url = ApiUrl + partialUrl

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  }).then((response) => {
    return response.json()
  })
}

export async function postAuthorized(partialUrl, payload) {
  const url = ApiUrl + partialUrl

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getToken()}`
    },
    body: JSON.stringify(payload)
  })
  .then((response) => {
    return response.json()
  });
}

export async function putAuthorized(partialUrl, payload) {
  const url = ApiUrl + partialUrl

  return fetch(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getToken()}`
    },
    body: JSON.stringify(payload)
  }).then((response) => {
    return response.json()
  })
}

export async function deleteAuthorized(partialUrl) {
  const url = ApiUrl + partialUrl

  return fetch(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getToken()}`
    }
  }).then((response) => {
    return response.json()
  })
}

export async function getAuthorized(partialUrl) {
  const url = ApiUrl + partialUrl

  return fetch(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getToken()}`
    }
  }).then((response) => {
    return response.json()
  })
}

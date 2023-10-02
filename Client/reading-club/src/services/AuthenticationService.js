import { postAnonymous } from '@/services/CrudService'

const key = 'book club'

function setToken(token) {
  localStorage.setItem(key, token)
}

function getToken() {
  return localStorage.getItem(key)
}

function removeToken() {
  localStorage.removeItem(key)
}

async function checkIfUserIsAuthenticated() {
  const token = getToken()

  const payload = {
    Token: token === null ? '' : token
  }

  const partialUrl = 'api/user/isTokenValid'

  const response = await postAnonymous(partialUrl, payload)

  if (response.status === true) {
    setToken(response.data)
  }

  return response.status
}

export { setToken, getToken, removeToken, checkIfUserIsAuthenticated }

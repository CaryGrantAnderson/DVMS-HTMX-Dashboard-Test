// Keep a failed panel visible in the connection notice until that panel recovers.
const failedRequests = new Set();
document.body.addEventListener('htmx:afterRequest', (event) => {
  const path = event.detail.requestConfig.path.split('?')[0];
  if (event.detail.successful) failedRequests.delete(path);
  else failedRequests.add(path);
  document.getElementById('request-error').hidden = failedRequests.size === 0;
});

function Toast({ message, type = "success", onClose }) {
  if (!message) return null;

  return (
    <div className={`toast toast--${type}`}>
      <div>{message}</div>
      <button className="toast__close" onClick={onClose}>
        ×
      </button>
    </div>
  );
}

export default Toast;
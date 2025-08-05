import React from 'react';
import { Modal, Button } from 'react-bootstrap';

export default function DeleteConfirmModal({ onCancel, onConfirm }) {
  return (
    <Modal show onHide={onCancel}>
      <Modal.Header closeButton>
        <Modal.Title>Delete article</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Are you sure you want to delete this article? This action cannot be undone.
      </Modal.Body>
      <Modal.Footer>
        <Button variant="light" onClick={onCancel}>Cancel</Button>
        <Button variant="primary" onClick={onConfirm}>Confirm</Button>
      </Modal.Footer>
    </Modal>
  );
}
